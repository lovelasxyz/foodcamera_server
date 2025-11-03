using MediatR;

namespace Cases.Application.Prizes.Queries.GetPrize;

public sealed record GetPrizeQuery(int PrizeId) : IRequest<PrizeDetailsDto?>;
