using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class CaseReadRepository : ICaseReadRepository
{
    private readonly CasesDbContext _dbContext;

    public CaseReadRepository(CasesDbContext dbContext)
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

        var cases = await query
            .OrderByDescending(@case => @case.IsActive)
            .ThenBy(@case => @case.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return cases;
    }

    public async Task<Case?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cases
            .AsNoTracking()
            .Include(@case => @case.CasePrizes)
                .ThenInclude(casePrize => casePrize.Prize)
            .SingleOrDefaultAsync(@case => @case.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }
}