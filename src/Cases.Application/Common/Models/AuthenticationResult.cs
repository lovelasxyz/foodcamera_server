using System;

namespace Cases.Application.Common.Models;

public sealed record AuthenticationResult(
    string AccessToken,
    DateTimeOffset TokenExpiresAt,
    Guid SessionId,
    DateTimeOffset SessionExpiresAt,
    AuthenticatedUserDto User);
