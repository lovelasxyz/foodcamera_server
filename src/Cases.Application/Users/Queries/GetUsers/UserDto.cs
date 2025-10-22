namespace Cases.Application.Users.Queries.GetUsers;

public sealed record UserDto(
    Guid Id,
    string? Name,
    decimal Balance,
    string Role,
    string? TelegramUsername,
    long? LastAuthAt);
