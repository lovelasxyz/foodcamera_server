using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Identity.Interfaces;

public interface IUserSessionWriteRepository
{
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    void Remove(UserSession session);
}
