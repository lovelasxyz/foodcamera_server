using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Prizes.Interfaces;

public interface IPrizeReadRepository
{
    Task<IReadOnlyList<Prize>> GetAsync(bool onlyActive, CancellationToken cancellationToken = default);
    Task<Prize?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}