using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Security;
using Cases.Domain.Enums;
using Cases.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cases.Infrastructure.Authentication.Session;

public sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserSessionService _sessionService;
    private readonly SessionSettings _sessionSettings;

    public SessionAuthenticationHandler(
        IUserSessionService sessionService,
        IOptions<SessionSettings> sessionOptions,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _sessionService = sessionService;
        _sessionSettings = sessionOptions.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var cookieName = SessionCookieOptionsFactory.ResolveCookieName(_sessionSettings);

        if (!Request.Cookies.TryGetValue(cookieName, out var cookieValue) || string.IsNullOrWhiteSpace(cookieValue))
        {
            return AuthenticateResult.NoResult();
        }

        if (!Guid.TryParse(cookieValue, out var sessionId))
        {
            return AuthenticateResult.Fail("Invalid session identifier.");
        }

        var session = await _sessionService.ValidateAsync(sessionId, Context.RequestAborted);

        if (session is null)
        {
            return AuthenticateResult.Fail("Session not found or expired.");
        }

        if (session.User is null)
        {
            return AuthenticateResult.Fail("Session user not loaded.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.UserId.ToString()),
            new(ClaimTypes.Role, session.User.Role.ToDatabaseValue()),
            new(SessionClaimTypes.SessionId, session.Id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(session.User.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, session.User.Name));
        }

        if (!string.IsNullOrWhiteSpace(session.User.TelegramId))
        {
            claims.Add(new Claim("telegram_id", session.User.TelegramId!));
        }

        if (!string.IsNullOrWhiteSpace(session.User.TelegramUsername))
        {
            claims.Add(new Claim("telegram_username", session.User.TelegramUsername!));
        }

        var identity = new ClaimsIdentity(claims, SessionAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SessionAuthenticationDefaults.AuthenticationScheme);

        var options = SessionCookieOptionsFactory.Create(_sessionSettings, session.ExpiresAt);
        Context.Response.Cookies.Append(cookieName, session.Id.ToString(), options);

        return AuthenticateResult.Success(ticket);
    }
}