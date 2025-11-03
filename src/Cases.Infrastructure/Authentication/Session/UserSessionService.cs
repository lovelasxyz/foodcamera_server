using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Identity.Interfaces;
using Cases.Domain.Entities;
using Cases.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Cases.Infrastructure.Authentication.Session;

public sealed class UserSessionService : IUserSessionService
{
    private readonly IUserSessionWriteRepository _sessions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SessionSettings _settings;

    public UserSessionService(
        IUserSessionWriteRepository sessions,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IOptions<SessionSettings> options)
    {
        _sessions = sessions;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _settings = options.Value;
    }

    public async Task<UserSession> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var expiresAt = now.AddMinutes(_settings.LifetimeMinutes);

        var session = UserSession.Create(user, now, expiresAt);

        await _sessions.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<UserSession?> ValidateAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdWithUserAsync(sessionId, cancellationToken);

        if (session is null) return null;

        var now = _dateTimeProvider.UtcNow;

        if (session.IsExpired(now))
        {
            _sessions.Remove(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return null;
        }

        if (_settings.SlidingExpiration)
        {
            var newExpiresAt = now.AddMinutes(_settings.LifetimeMinutes);
            session.Refresh(now, newExpiresAt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return session;
    }

    public async Task InvalidateAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return;
        }

        _sessions.Remove(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}