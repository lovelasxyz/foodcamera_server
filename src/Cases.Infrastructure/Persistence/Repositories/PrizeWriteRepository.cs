using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class PrizeWriteRepository : IPrizeWriteRepository
{
    private readonly CasesDbContext _dbContext;

    public PrizeWriteRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Prize prize, CancellationToken cancellationToken = default)
    {
        await _dbContext.Prizes.AddAsync(prize, cancellationToken).ConfigureAwait(false);
    }

    public Task<Prize?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Prizes.FirstOrDefaultAsync(prize => prize.Id == id, cancellationToken);
    }

    public async Task<bool> UniqueKeyExistsAsync(string uniqueKey, int? excludePrizeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uniqueKey))
        {
            return false;
        }

        var query = _dbContext.Prizes.AsQueryable();

        if (excludePrizeId.HasValue)
        {
            query = query.Where(prize => prize.Id != excludePrizeId.Value);
        }

        return await query.AnyAsync(prize => prize.UniqueKey == uniqueKey, cancellationToken).ConfigureAwait(false);
    }
}
