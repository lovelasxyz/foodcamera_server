using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Security;
using Microsoft.AspNetCore.Http;

namespace Cases.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => TryParseGuid(GetClaimValue(ClaimTypes.NameIdentifier));

    public Guid? SessionId => TryParseGuid(GetClaimValue(SessionClaimTypes.SessionId));

    public string? TelegramId => GetClaimValue("telegram_id");

    public string? TelegramUsername => GetClaimValue("telegram_username");

    public IReadOnlyCollection<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
        .Select(claim => claim.Value)
        .ToArray() ?? Array.Empty<string>();

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    private static Guid? TryParseGuid(string? value)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }
}
