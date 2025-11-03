using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Prizes.Queries.GetPrizes;

public sealed class GetPrizesQueryHandler : IRequestHandler<GetPrizesQuery, IReadOnlyList<PrizeListItemDto>>
{
    private readonly IPrizeReadRepository _prizes;

    public GetPrizesQueryHandler(IPrizeReadRepository prizes)
    {
        _prizes = prizes;
    }

    public async Task<IReadOnlyList<PrizeListItemDto>> Handle(GetPrizesQuery request, CancellationToken cancellationToken)
    {
        var prizeEntities = await _prizes.GetAsync(request.OnlyActive, cancellationToken);

        return prizeEntities
            .Select(p => new PrizeListItemDto(
                p.Id,
                p.Name,
                p.Price,
                p.Rarity.ToDatabaseValue(),
                p.IsActive,
                p.IsShard,
                p.ShardKey,
                p.DropWeight))
            .ToList();
    }
}
