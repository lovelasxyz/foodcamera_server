using System.Collections.Generic;

namespace Cases.Application.Cases.Queries.GetCase;

public sealed record CaseDto(
    int Id,
    string? Name,
    string? Image,
    decimal Price,
    decimal CommissionPercent,
    bool IsActive,
    int SortOrder,
    decimal Balance,
    DateTimeOffset VisibleFrom,
    DateTimeOffset? VisibleUntil,
    bool AutoHide,
    IReadOnlyList<CasePrizeDto> Prizes);
