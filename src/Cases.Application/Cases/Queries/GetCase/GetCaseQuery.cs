using MediatR;

namespace Cases.Application.Cases.Queries.GetCase;

public sealed record GetCaseQuery(int CaseId) : IRequest<CaseDto>;
