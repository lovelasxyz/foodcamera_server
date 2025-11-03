using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Identity.Interfaces;

public interface IUserSessionReadRepository
{
    Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default);
}