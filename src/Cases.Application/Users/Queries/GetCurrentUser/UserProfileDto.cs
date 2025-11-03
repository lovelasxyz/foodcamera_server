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
    string Status);

public sealed record UserPerksDto(
    bool? FreeSpins,
    bool? UnlimitedBalance);
