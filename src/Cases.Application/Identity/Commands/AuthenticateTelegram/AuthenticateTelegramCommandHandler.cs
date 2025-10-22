using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cases.Application.Identity.Commands.AuthenticateTelegram;

public sealed class AuthenticateTelegramCommandHandler : IRequestHandler<AuthenticateTelegramCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITelegramAuthService _telegramAuthService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticateTelegramCommandHandler(
        IApplicationDbContext dbContext,
        ITelegramAuthService telegramAuthService,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _telegramAuthService = telegramAuthService;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthenticationResult> Handle(AuthenticateTelegramCommand request, CancellationToken cancellationToken)
    {
        var telegramUser = _telegramAuthService.ValidateAndParse(request.InitData);
        var now = _dateTimeProvider.UtcNow;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.TelegramId == telegramUser.Id, cancellationToken);

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

            await _dbContext.Users.AddAsync(user, cancellationToken);
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

        await _dbContext.SaveChangesAsync(cancellationToken);

        var tokenResult = _tokenService.GenerateToken(user);

        return new AuthenticationResult(
            tokenResult.Token,
            tokenResult.ExpiresAt,
            new AuthenticatedUserDto(
                user.Id,
                user.Name,
                user.Role,
                user.TelegramUsername,
                user.Balance));
    }
}
