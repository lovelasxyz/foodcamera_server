using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Cases.Interfaces;

public interface ICaseReadRepository
{
    Task<IReadOnlyList<Case>> GetAsync(bool onlyActive, CancellationToken cancellationToken = default);
    Task<Case?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}