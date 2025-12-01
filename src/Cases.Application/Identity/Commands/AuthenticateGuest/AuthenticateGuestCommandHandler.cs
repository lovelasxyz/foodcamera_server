using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Application.Users.Interfaces;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Identity.Commands.AuthenticateGuest;

public sealed class AuthenticateGuestCommandHandler : IRequestHandler<AuthenticateGuestCommand, AuthenticationResult>
{
    private readonly IUserWriteRepository _users;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserSessionService _userSessionService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticateGuestCommandHandler(
        IUserWriteRepository users,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IUserSessionService userSessionService,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _userSessionService = userSessionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> Handle(AuthenticateGuestCommand request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        // Always create a new guest user
        var user = User.CreateGuest(now);

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tokenResult = _tokenService.GenerateToken(user);
        var session = await _userSessionService.CreateAsync(user, cancellationToken);

        var userDto = new AuthenticatedUserDto(
            user.Id,
            user.Name,
            user.Role.ToString(),
            user.TelegramUsername,
            user.Balance);

        return new AuthenticationResult(
            tokenResult.Token,
            tokenResult.ExpiresAt,
            session.Id,
            session.ExpiresAt,
            userDto);
    }
}
