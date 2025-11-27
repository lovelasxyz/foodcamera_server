using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class CaseRepository : ICaseReadRepository, ICaseWriteRepository
{
    private readonly CasesDbContext _dbContext;

    public CaseRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Case>> GetAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        IQueryable<Case> query = _dbContext.Cases
            .AsNoTracking()
            .Include(@case => @case.CasePrizes);

        if (!includeInactive)
        {
            query = query.Where(@case => @case.IsActive);
        }

        return await query
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Case>> GetWithPrizesAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        IQueryable<Case> query = _dbContext.Cases
            .AsNoTracking()
            .Include(@case => @case.CasePrizes)
                .ThenInclude(casePrize => casePrize.Prize);

        if (!includeInactive)
        {
            query = query.Where(@case => @case.IsActive);
        }

        return await query
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Case?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cases
            .AsTracking()
            .Include(@case => @case.CasePrizes)
                .ThenInclude(casePrize => casePrize.Prize)
            .SingleOrDefaultAsync(@case => @case.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Case caseEntity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cases.AddAsync(caseEntity, cancellationToken).ConfigureAwait(false);
    }

    public Task<Case?> GetByIdWithPrizesAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Cases
            .Include(@case => @case.CasePrizes)
            .SingleOrDefaultAsync(@case => @case.Id == id, cancellationToken);
    }

    public void Remove(Case caseEntity)
    {
        _dbContext.Cases.Remove(caseEntity);
    }

    public async Task AddPrizeAsync(CasePrize casePrize, CancellationToken cancellationToken = default)
    {
        await _dbContext.CasePrizes.AddAsync(casePrize, cancellationToken).ConfigureAwait(false);
    }

    public Task<CasePrize?> GetCasePrizeAsync(int caseId, int prizeId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CasePrizes
            .FirstOrDefaultAsync(cp => cp.CaseId == caseId && cp.PrizeId == prizeId, cancellationToken);
    }

    public void RemovePrize(CasePrize casePrize)
    {
        _dbContext.CasePrizes.Remove(casePrize);
    }
}
