using MediatR;

namespace Cases.Application.Cases.Commands.PublishCase;

public sealed record PublishCaseCommand(int CaseId, DateTimeOffset? VisibleFrom, DateTimeOffset? VisibleUntil) : IRequest<Unit>;
