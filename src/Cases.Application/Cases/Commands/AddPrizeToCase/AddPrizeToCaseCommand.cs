using MediatR;

namespace Cases.Application.Cases.Commands.AddPrizeToCase;

public sealed record AddPrizeToCaseCommand(int CaseId, int PrizeId, int Weight) : IRequest<int>;
