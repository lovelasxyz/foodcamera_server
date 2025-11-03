namespace Cases.Application.Prizes.Queries.GetPrizes;

public sealed record PrizeListItemDto(
    int Id,
    string? Name,
    decimal Price,
    string Rarity,
    bool IsActive,
    bool IsShard,
    string? ShardKey,
    decimal DropWeight);
