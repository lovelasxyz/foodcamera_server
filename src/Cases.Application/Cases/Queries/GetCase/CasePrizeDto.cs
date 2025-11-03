namespace Cases.Application.Cases.Queries.GetCase;

public sealed record CasePrizeDto(
    int PrizeId,
    string? PrizeName,
    decimal PrizePrice,
    string? PrizeRarity,
    int Weight,
    bool PrizeIsActive);
