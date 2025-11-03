using MediatR;

namespace Cases.Application.Cases.Commands.CreateCase;

public sealed record CreateCaseCommand(
    string? Name,
    string? Image,
    decimal Price,
    decimal CommissionPercent,
    int SortOrder,
    bool AutoHide,
    DateTimeOffset? VisibleFrom,
    DateTimeOffset? VisibleUntil) : IRequest<int>;
