using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Application.Users.Interfaces;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Identity.Commands.AuthenticateTelegram;

public sealed class AuthenticateTelegramCommandHandler : IRequestHandler<AuthenticateTelegramCommand, AuthenticationResult>
{
    private readonly IUserWriteRepository _users;
    private readonly ITelegramAuthService _telegramAuthService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserSessionService _userSessionService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticateTelegramCommandHandler(
        IUserWriteRepository users,
        ITelegramAuthService telegramAuthService,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IUserSessionService userSessionService,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _telegramAuthService = telegramAuthService;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _userSessionService = userSessionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> Handle(AuthenticateTelegramCommand request, CancellationToken cancellationToken)
    {
        var telegramUser = _telegramAuthService.ValidateAndParse(request.InitData);
        var now = _dateTimeProvider.UtcNow;

        var user = await _users.GetByTelegramIdAsync(telegramUser.Id, cancellationToken);

        if (user is null)
        {
            user = User.CreateFromTelegram(
                telegramUser.Id,
                telegramUser.FirstName,
                telegramUser.LastName,
                telegramUser.Username,
                telegramUser.PhotoUrl,
                telegramUser.AuthDate,
                now);

            await _users.AddAsync(user, cancellationToken);
        }
        else
        {
            user.UpdateTelegramProfile(
                telegramUser.FirstName,
                telegramUser.LastName,
                telegramUser.Username,
                telegramUser.PhotoUrl,
                telegramUser.AuthDate,
                now);
        }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tokenResult = _tokenService.GenerateToken(user);
        var session = await _userSessionService.CreateAsync(user, cancellationToken);

        return new AuthenticationResult(
            tokenResult.Token,
            tokenResult.ExpiresAt,
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
