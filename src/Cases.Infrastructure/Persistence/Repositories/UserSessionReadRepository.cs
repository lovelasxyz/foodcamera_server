using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Identity.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class UserSessionReadRepository : IUserSessionReadRepository
{
    private readonly CasesDbContext _dbContext;

    public UserSessionReadRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions
            .AsNoTracking()
            .Include(session => session.User)
            .SingleOrDefaultAsync(session => session.Id == sessionId, cancellationToken)
            .ConfigureAwait(false);
    }
}