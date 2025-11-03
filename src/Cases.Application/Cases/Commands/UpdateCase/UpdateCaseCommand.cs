using MediatR;

namespace Cases.Application.Cases.Commands.UpdateCase;

public sealed record UpdateCaseCommand(
    int CaseId,
    bool NameSpecified,
    string? Name,
    bool ImageSpecified,
    string? Image,
    bool PriceSpecified,
    decimal? Price,
    bool CommissionPercentSpecified,
    decimal? CommissionPercent,
    bool SortOrderSpecified,
    int? SortOrder,
    bool AutoHideSpecified,
    bool? AutoHide,
    bool VisibleFromSpecified,
    DateTimeOffset? VisibleFrom,
    bool VisibleUntilSpecified,
    DateTimeOffset? VisibleUntil) : IRequest<Unit>;
