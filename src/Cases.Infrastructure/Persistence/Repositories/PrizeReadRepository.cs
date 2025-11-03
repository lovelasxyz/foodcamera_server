using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class PrizeReadRepository : IPrizeReadRepository
{
    private readonly CasesDbContext _dbContext;

    public PrizeReadRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Prize>> GetAsync(bool onlyActive, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Prizes.AsNoTracking();

        if (onlyActive)
        {
            query = query.Where(prize => prize.IsActive);
        }

        var items = await query
            .OrderByDescending(prize => prize.IsActive)
            .ThenBy(prize => prize.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return items;
    }

    public async Task<Prize?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Prizes
            .AsNoTracking()
            .SingleOrDefaultAsync(prize => prize.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }
}