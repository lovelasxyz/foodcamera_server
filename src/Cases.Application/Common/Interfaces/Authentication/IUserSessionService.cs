using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Common.Interfaces.Authentication;

public interface IUserSessionService
{
    Task<UserSession> CreateAsync(User user, CancellationToken cancellationToken);
    Task<UserSession?> ValidateAsync(Guid sessionId, CancellationToken cancellationToken);
    Task InvalidateAsync(Guid sessionId, CancellationToken cancellationToken);
}