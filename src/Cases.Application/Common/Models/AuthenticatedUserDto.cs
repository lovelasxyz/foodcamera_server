namespace Cases.Application.Common.Models;

public sealed record AuthenticatedUserDto(
    Guid Id,
    string? Name,
    string Role,
    string? TelegramUsername,
    decimal Balance);
