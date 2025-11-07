using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Queries.GetCase;
using Cases.Application.Cases.Queries.GetCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[AllowAnonymous]
public sealed class CasesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaseResponse>>> GetCases(CancellationToken cancellationToken)
    {
    var cases = await Mediator.Send(new GetCasesQuery(null, IncludeInactive: false), cancellationToken);

        if (cases.Count == 0)
        {
            return Ok(Array.Empty<CaseResponse>());
        }

        var result = new List<CaseResponse>(cases.Count);

        foreach (var item in cases)
        {
            var caseDto = await Mediator.Send(new GetCaseQuery(item.Id), cancellationToken);
            result.Add(MapCase(caseDto));
        }

        return Ok(result);
    }

    private static CaseResponse MapCase(CaseDto source)
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
