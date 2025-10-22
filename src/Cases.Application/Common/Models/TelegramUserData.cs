namespace Cases.Application.Common.Models;

public sealed record TelegramUserData(
    string Id,
    string? FirstName,
    string? LastName,
    string? Username,
    string? PhotoUrl,
    long AuthDate);
