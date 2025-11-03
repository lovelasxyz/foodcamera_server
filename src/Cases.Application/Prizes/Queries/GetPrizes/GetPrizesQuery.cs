using System.Collections.Generic;
using MediatR;

namespace Cases.Application.Prizes.Queries.GetPrizes;

public sealed record GetPrizesQuery(bool OnlyActive = true) : IRequest<IReadOnlyList<PrizeListItemDto>>;
