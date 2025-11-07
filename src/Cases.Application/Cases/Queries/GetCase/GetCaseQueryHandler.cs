using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Interfaces;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Queries.GetCase;

public sealed class GetCaseQueryHandler : IRequestHandler<GetCaseQuery, CaseDto>
{
    private readonly ICaseReadRepository _cases;

    public GetCaseQueryHandler(ICaseReadRepository cases)
    {
        _cases = cases;
    }

    public async Task<CaseDto> Handle(GetCaseQuery request, CancellationToken cancellationToken)
    {
        var @case = await _cases.GetByIdAsync(request.CaseId, cancellationToken);

        if (@case is null)
        {
            throw new NotFoundException(nameof(Case), request.CaseId);
        }

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
