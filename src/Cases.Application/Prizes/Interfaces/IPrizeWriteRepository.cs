using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Prizes.Interfaces;

public interface IPrizeWriteRepository
{
    Task AddAsync(Prize prize, CancellationToken cancellationToken = default);
    Task<Prize?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UniqueKeyExistsAsync(string uniqueKey, int? excludePrizeId = null, CancellationToken cancellationToken = default);
}
