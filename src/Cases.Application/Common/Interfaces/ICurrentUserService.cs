using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cases.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? SessionId { get; }
    string? TelegramId { get; }
    string? TelegramUsername { get; }
    IReadOnlyCollection<string> Roles { get; }
    ClaimsPrincipal? User { get; }
}
