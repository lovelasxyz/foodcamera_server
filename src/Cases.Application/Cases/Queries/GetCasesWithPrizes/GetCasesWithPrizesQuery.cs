using System.Collections.Generic;
using Cases.Application.Cases.Queries.GetCase;
using MediatR;

namespace Cases.Application.Cases.Queries.GetCasesWithPrizes;

public sealed record GetCasesWithPrizesQuery(bool IncludeInactive = true) : IRequest<IReadOnlyList<CaseDto>>;
