using MediatR;

namespace Cases.Application.Cases.Commands.FreezeCase;

public sealed record FreezeCaseCommand(int CaseId) : IRequest<Unit>;
