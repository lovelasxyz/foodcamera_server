using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Interfaces;
using Cases.Domain.Entities;
using MediatR;

namespace Cases.Application.Cases.Queries.GetCases;

public sealed class GetCasesQueryHandler : IRequestHandler<GetCasesQuery, IReadOnlyList<CaseListItemDto>>
{
    private readonly ICaseReadRepository _cases;

    public GetCasesQueryHandler(ICaseReadRepository cases)
    {
        _cases = cases;
    }

    public async Task<IReadOnlyList<CaseListItemDto>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
    {
        var caseEntities = await _cases.GetAsync(request.IncludeInactive, cancellationToken);

        IEnumerable<Case> filtered = request.IncludeInactive
            ? caseEntities
            : caseEntities.Where(@case => @case.IsActive);

        var ordered = filtered
            .OrderByDescending(@case => @case.IsActive)
            .ThenBy(@case => @case.SortOrder);

        var limited = request.Limit is > 0
            ? ordered.Take(request.Limit.Value)
            : ordered;

        return limited
            .Select(@case => new CaseListItemDto(
                @case.Id,
                @case.Name,
                @case.IsActive,
                @case.Price,
                @case.SortOrder,
                @case.CasePrizes.Count))
            .ToList();
    }
}
