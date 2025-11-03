using System.Collections.Generic;
using MediatR;

namespace Cases.Application.Cases.Queries.GetCases;

public sealed record GetCasesQuery(int? Limit = null, bool IncludeInactive = true) : IRequest<IReadOnlyList<CaseListItemDto>>;
