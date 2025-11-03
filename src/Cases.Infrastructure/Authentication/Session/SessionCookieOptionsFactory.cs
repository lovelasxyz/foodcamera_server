using System;
using Cases.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;

namespace Cases.Infrastructure.Authentication.Session;

public static class SessionCookieOptionsFactory
{
    public static string ResolveCookieName(SessionSettings settings) =>
        string.IsNullOrWhiteSpace(settings.CookieName)
            ? SessionAuthenticationDefaults.SessionCookieName
            : settings.CookieName;

    public static CookieOptions Create(SessionSettings settings, DateTimeOffset expiresAt)
    {
        var options = new CookieOptions
        {
            HttpOnly = settings.HttpOnly,
            Secure = settings.Secure,
            SameSite = ParseSameSite(settings.SameSite),
            Path = string.IsNullOrWhiteSpace(settings.Path) ? "/" : settings.Path,
            Expires = expiresAt.UtcDateTime
        };

        if (expiresAt > DateTimeOffset.UtcNow)
        {
            options.MaxAge = expiresAt - DateTimeOffset.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(settings.Domain))
        {
            options.Domain = settings.Domain;
        }

        return options;
    }

    public static CookieOptions CreateExpired(SessionSettings settings)
    {
        var options = Create(settings, DateTimeOffset.UtcNow.AddDays(-1));
        options.MaxAge = TimeSpan.Zero;
        return options;
    }

    private static SameSiteMode ParseSameSite(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "none" => SameSiteMode.None,
            "strict" => SameSiteMode.Strict,
            _ => SameSiteMode.Lax
        };
}
