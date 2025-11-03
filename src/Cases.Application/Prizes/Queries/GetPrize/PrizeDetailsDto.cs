namespace Cases.Application.Prizes.Queries.GetPrize;

public sealed record PrizeDetailsDto(
    int Id,
    string? Name,
    decimal Price,
    string? Image,
    string Rarity,
    bool IsActive,
    bool IsShard,
    string? ShardKey,
    int? ShardsRequired,
    string? Description,
    string? UniqueKey,
    bool Stackable,
    bool NotAwardIfOwned,
    bool NonRemovableGift,
    string? BenefitType,
    string? BenefitDataJson,
    decimal DropWeight,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
