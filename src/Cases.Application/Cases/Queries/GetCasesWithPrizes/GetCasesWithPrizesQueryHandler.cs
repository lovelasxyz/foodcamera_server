using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Interfaces;
using Cases.Application.Cases.Queries.GetCase;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Cases.Queries.GetCasesWithPrizes;

public sealed class GetCasesWithPrizesQueryHandler : IRequestHandler<GetCasesWithPrizesQuery, IReadOnlyList<CaseDto>>
{
    private readonly ICaseReadRepository _cases;

    public GetCasesWithPrizesQueryHandler(ICaseReadRepository cases)
    {
        _cases = cases;
    }

    public async Task<IReadOnlyList<CaseDto>> Handle(GetCasesWithPrizesQuery request, CancellationToken cancellationToken)
    {
        var caseEntities = await _cases.GetWithPrizesAsync(request.IncludeInactive, cancellationToken).ConfigureAwait(false);

        return caseEntities
            .OrderByDescending(@case => @case.IsActive)
            .ThenBy(@case => @case.SortOrder)
            .Select(MapCase)
            .ToList();
    }

    private static CaseDto MapCase(Domain.Entities.Case @case)
    {
        var prizes = @case.CasePrizes
            .OrderBy(cp => cp.Weight)
            .Select(cp => new CasePrizeDto(
                cp.PrizeId,
                cp.Prize.Name,
                cp.Prize.Price,
                cp.Prize.Rarity.ToDatabaseValue(),
                cp.Weight,
                cp.Prize.IsActive,
                cp.Prize.Image,
                cp.Prize.IsShard,
                cp.Prize.ShardKey,
                cp.Prize.ShardsRequired,
                cp.Prize.Description,
                cp.Prize.UniqueKey,
                cp.Prize.Stackable,
                cp.Prize.NotAwardIfOwned,
                cp.Prize.NonRemovableGift,
                cp.Prize.BenefitType.ToDatabaseValue(),
                cp.Prize.BenefitDataJson))
            .ToList();

        return new CaseDto(
            @case.Id,
            @case.Name,
            @case.Image,
            @case.Price,
            @case.CommissionPercent,
            @case.IsActive,
            @case.SortOrder,
            @case.Balance,
            @case.VisibleFrom,
            @case.VisibleUntil,
            @case.AutoHide,
            @case.UpdatedAt,
            prizes);
    }
}
