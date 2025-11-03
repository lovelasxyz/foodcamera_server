using System;

namespace Cases.Application.Common.Models;

public sealed record SessionStateResult(
    Guid SessionId,
    DateTimeOffset SessionExpiresAt,
    AuthenticatedUserDto User);
