namespace Cases.Application.Common.Models;

public sealed record AuthenticationResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    AuthenticatedUserDto User);
