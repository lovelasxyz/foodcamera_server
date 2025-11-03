using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using MediatR;

namespace Cases.Application.Identity.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserSessionService _sessionService;

    public LogoutCommandHandler(
        ICurrentUserService currentUserService,
        IUserSessionService sessionService)
    {
        _currentUserService = currentUserService;
        _sessionService = sessionService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var sessionId = _currentUserService.SessionId;

        if (sessionId.HasValue)
        {
            await _sessionService.InvalidateAsync(sessionId.Value, cancellationToken);
        }

        return Unit.Value;
    }
}
