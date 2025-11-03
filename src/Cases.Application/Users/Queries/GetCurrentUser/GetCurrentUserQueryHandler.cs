using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Users.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    private readonly IUserReadRepository _users;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(IUserReadRepository users, ICurrentUserService currentUserService)
    {
        _users = users;
        _currentUserService = currentUserService;
    }

    public async Task<UserProfileDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _users.GetByIdAsync(userId.Value, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"User '{userId}' was not found.");
        }

        var role = user.Role.ToDatabaseValue();
        var isAdmin = user.Role == UserRole.Admin;
        var status = role; // Aligns with client expectations (regular, premium, advertiser, etc.)

        var stats = new UserStatsDto(0, user.LastAuthAt);

        UserTelegramDto? telegram = null;
        if (!string.IsNullOrWhiteSpace(user.TelegramId))
        {
            var registeredAt = user.TelegramRegisteredAt ?? user.LastAuthAt ?? 0;
            telegram = new UserTelegramDto(
                user.TelegramId,
                user.TelegramUsername,
                user.TelegramHasPhoto,
                user.TelegramPhotoUrl,
                registeredAt);
        }

        return new UserProfileDto(
            user.Id.ToString(),
            user.Name,
            user.TelegramUsername,
            user.TelegramPhotoUrl ?? string.Empty,
            user.Balance,
            status,
            isAdmin,
            stats,
            telegram,
            Array.Empty<UserInventoryItemDto>(),
            new Dictionary<string, int>(),
            new Dictionary<string, long>(),
            null);
    }
}
