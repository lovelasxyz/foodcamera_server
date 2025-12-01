using System.Collections.Generic;

namespace Cases.Application.Users.Queries.GetCurrentUser;

public sealed record UserProfileDto(
    string Id,
    string? Name,
    string? Username,
    string? Avatar,
    decimal Balance,
    string Status,
    bool IsAdmin,
    UserStatsDto Stats,
    UserTelegramDto? Telegram,
    IReadOnlyList<UserInventoryItemDto> Inventory,
    IReadOnlyDictionary<string, int> Shards,
    IReadOnlyDictionary<string, long> ShardUpdatedAt,
    UserPerksDto? Perks);

public sealed record UserStatsDto(int SpinsCount, long? LastAuthAt);

public sealed record UserTelegramDto(
    string Id,
    string? Username,
    bool HasPhoto,
    string? PhotoUrl,
    long RegisteredAt);

public sealed record UserInventoryItemDto(
    string Id,
    int PrizeId,
    string FromCase,
    long ObtainedAt,
    string Status,
    UserPrizeDto? Prize);

public sealed record UserPrizeDto(
    int Id,
    string Name,
    decimal Price,
    string? Image,
    string Rarity,
    bool IsShard,
    string? ShardKey,
    int? ShardsRequired,
    string? Description,
    string? Benefit,
    string? UniqueKey,
    bool Stackable,
    bool NotAwardIfOwned,
    bool NonRemovableGift
);

public sealed record UserPerksDto(
    bool? FreeSpins,
    bool? UnlimitedBalance);
