namespace Cases.Application.Cases.Queries.GetCase;

public sealed record CasePrizeDto(
    int PrizeId,
    string? PrizeName,
    decimal PrizePrice,
    string? PrizeRarity,
    int Weight,
    bool PrizeIsActive,
    string? PrizeImage,
    bool PrizeIsShard,
    string? PrizeShardKey,
    int? PrizeShardsRequired,
    string? PrizeDescription,
    string? PrizeUniqueKey,
    bool PrizeStackable,
    bool PrizeNotAwardIfOwned,
    bool PrizeNonRemovableGift,
    string? PrizeBenefitType,
    string? PrizeBenefitDataJson);
