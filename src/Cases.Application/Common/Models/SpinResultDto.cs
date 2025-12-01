namespace Cases.Application.Common.Models;

public sealed record SpinResultDto(
    PrizeDto Prize,
    AwardOutcomeDto Outcome,
    decimal UserBalance,
    IEnumerable<InventoryItemDto> UserInventory);

public sealed record PrizeDto(
    string Id,
    string Name,
    string Type,
    string Rarity,
    string? ImageUrl,
    decimal? Amount);

public sealed record AwardOutcomeDto(
    bool Success,
    string Message,
    string? TransactionId);

public sealed record InventoryItemDto(
    string Id,
    string PrizeId,
    string Status,
    DateTimeOffset AcquiredAt,
    PrizeDto? Prize);
