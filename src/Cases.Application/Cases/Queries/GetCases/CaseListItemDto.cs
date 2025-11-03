namespace Cases.Application.Cases.Queries.GetCases;

public sealed record CaseListItemDto(
    int Id,
    string? Name,
    bool IsActive,
    decimal Price,
    int SortOrder,
    int PrizesCount);
