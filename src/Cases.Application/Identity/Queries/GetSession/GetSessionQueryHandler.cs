using System;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Models;
using Cases.Application.Identity.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Identity.Queries.GetSession;

public sealed class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, SessionStateResult>
{
    private readonly IUserSessionReadRepository _sessions;
    private readonly ICurrentUserService _currentUserService;

    public GetSessionQueryHandler(
        IUserSessionReadRepository sessions,
        ICurrentUserService currentUserService)
    {
        _sessions = sessions;
        _currentUserService = currentUserService;
    }

    public async Task<SessionStateResult> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedAccessException("User context is not available.");
        var sessionId = _currentUserService.SessionId
                        ?? throw new UnauthorizedAccessException("Session identifier is missing.");

        var session = await _sessions.GetByIdWithUserAsync(sessionId, cancellationToken);

        if (session is null || session.User is null)
        {
            throw new UnauthorizedAccessException("Session not found.");
        }

        var user = session.User;

        return new SessionStateResult(
            session.Id,
            session.ExpiresAt,
            new AuthenticatedUserDto(
                user.Id,
                user.Name,
                user.Role.ToDatabaseValue(),
                user.TelegramUsername,
                user.Balance));
    }
}
