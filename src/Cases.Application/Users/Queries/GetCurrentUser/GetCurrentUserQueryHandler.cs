using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Users.Interfaces;
using Cases.Application.Inventory.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    private readonly IUserReadRepository _users;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserInventoryRepository _inventory;

    public GetCurrentUserQueryHandler(
        IUserReadRepository users,
        ICurrentUserService currentUserService,
        IUserInventoryRepository inventory)
    {
        _users = users;
        _currentUserService = currentUserService;
        _inventory = inventory;
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

        var inventoryItems = await _inventory.GetByUserIdAsync(user.Id, cancellationToken);
        var inventoryDtos = inventoryItems.Select(i => new UserInventoryItemDto(
            i.InventoryItemId,
            i.PrizeId,
            i.FromCase ?? "Unknown",
            i.ObtainedAt,
            i.Status.ToString().ToLowerInvariant(),
            new UserPrizeDto(
                i.Prize.Id,
                i.Prize.Name ?? "Unknown Prize",
                i.Prize.Price,
                i.Prize.Image ?? "/assets/images/placeholder.png",
                i.Prize.Rarity.ToString().ToLowerInvariant(),
                i.Prize.IsShard,
                i.Prize.ShardKey,
                i.Prize.ShardsRequired,
                i.Prize.Description,
                null, // Benefit mapping skipped for simplicity
                i.Prize.UniqueKey,
                i.Prize.Stackable,
                i.Prize.NotAwardIfOwned,
                i.Prize.NonRemovableGift
            )
        )).ToList();

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
            inventoryDtos,
            new Dictionary<string, int>(),
            new Dictionary<string, long>(),
            null);
    }
}
