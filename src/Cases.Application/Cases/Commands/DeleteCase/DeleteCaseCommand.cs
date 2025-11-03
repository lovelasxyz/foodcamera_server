using MediatR;

namespace Cases.Application.Cases.Commands.DeleteCase;

public sealed record DeleteCaseCommand(int CaseId) : IRequest<Unit>;
