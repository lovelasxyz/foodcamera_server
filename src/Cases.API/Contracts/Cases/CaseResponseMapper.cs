using System;
using System.Collections.Generic;
using System.Linq;
using Cases.Application.Cases.Queries.GetCase;

namespace Cases.API.Contracts.Cases;

public static class CaseResponseMapper
{
    public static IReadOnlyList<CaseResponse> MapMany(IEnumerable<CaseDto> cases)
    {
        return cases.Select(Map).ToList();
    }

    public static CaseResponse Map(CaseDto source)
    {
        var prizes = source.Prizes
            .Select(prize => new CasePrizeResponse(
                prize.PrizeId,
                prize.PrizeName ?? string.Empty,
                prize.PrizePrice,
                prize.PrizeRarity ?? "common",
                prize.PrizeImage ?? string.Empty,
                prize.Weight,
                prize.PrizeIsShard,
                prize.PrizeShardKey,
                prize.PrizeShardsRequired,
                prize.PrizeDescription,
                prize.PrizeUniqueKey,
                prize.PrizeStackable,
                prize.PrizeNotAwardIfOwned,
                prize.PrizeNonRemovableGift,
                prize.PrizeBenefitType,
                prize.PrizeBenefitDataJson))
            .ToList();

        return new CaseResponse(
            source.Id,
            source.Name ?? string.Empty,
            source.Price,
            source.Image,
            source.IsActive,
            prizes,
            source.UpdatedAt);
    }
}

public sealed record CaseResponse(
    int Id,
    string Name,
    decimal Price,
    string? Image,
    bool IsActive,
    IReadOnlyList<CasePrizeResponse> Prizes,
    DateTimeOffset UpdatedAt);

public sealed record CasePrizeResponse(
    int Id,
    string Name,
    decimal Price,
    string Rarity,
    string Image,
    int? Weight,
    bool IsShard,
    string? ShardKey,
    int? ShardsRequired,
    string? Description,
    string? UniqueKey,
    bool Stackable,
    bool NotAwardIfOwned,
    bool NonRemovableGift,
    string? BenefitType,
    string? BenefitDataJson);
