using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Prizes.Queries.GetPrize;

public sealed class GetPrizeQueryHandler : IRequestHandler<GetPrizeQuery, PrizeDetailsDto?>
{
    private readonly IPrizeReadRepository _prizes;

    public GetPrizeQueryHandler(IPrizeReadRepository prizes)
    {
        _prizes = prizes;
    }

    public async Task<PrizeDetailsDto?> Handle(GetPrizeQuery request, CancellationToken cancellationToken)
    {
        var prize = await _prizes.GetByIdAsync(request.PrizeId, cancellationToken);

        if (prize is null)
        {
            return null;
        }

        return new PrizeDetailsDto(
            prize.Id,
            prize.Name,
            prize.Price,
            prize.Image,
            prize.Rarity.ToDatabaseValue(),
            prize.IsActive,
            prize.IsShard,
            prize.ShardKey,
            prize.ShardsRequired,
            prize.Description,
            prize.UniqueKey,
            prize.Stackable,
            prize.NotAwardIfOwned,
            prize.NonRemovableGift,
            prize.BenefitType.ToDatabaseValue(),
            prize.BenefitDataJson,
            prize.DropWeight,
            prize.CreatedAt,
            prize.UpdatedAt);
    }
}
