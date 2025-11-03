using MediatR;

namespace Cases.Application.Prizes.Commands.CreatePrize;

public sealed record CreatePrizeCommand(
    string? Name,
    decimal Price,
    string? Image,
    string Rarity,
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
    decimal DropWeight) : IRequest<int>;
