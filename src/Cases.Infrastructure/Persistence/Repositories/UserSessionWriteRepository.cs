using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Identity.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class UserSessionWriteRepository : IUserSessionWriteRepository
{
    private readonly CasesDbContext _dbContext;

    public UserSessionWriteRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserSessions.AddAsync(session, cancellationToken).ConfigureAwait(false);
    }

    public Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserSessions
            .Include(session => session.User)
            .FirstOrDefaultAsync(session => session.Id == sessionId, cancellationToken);
    }

    public Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserSessions.FirstOrDefaultAsync(session => session.Id == sessionId, cancellationToken);
    }

    public void Remove(UserSession session)
    {
        _dbContext.UserSessions.Remove(session);
    }
}
