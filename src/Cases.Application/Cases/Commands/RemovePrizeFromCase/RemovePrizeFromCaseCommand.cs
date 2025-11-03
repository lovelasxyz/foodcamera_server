using MediatR;

namespace Cases.Application.Cases.Commands.RemovePrizeFromCase;

public sealed record RemovePrizeFromCaseCommand(int CaseId, int PrizeId) : IRequest<Unit>;
