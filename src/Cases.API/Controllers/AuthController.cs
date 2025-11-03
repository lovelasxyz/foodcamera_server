using System;
using System.Threading;
using Cases.Application.Common.Models;
using Cases.Application.Identity.Commands.AuthenticateTelegram;
using Cases.Application.Identity.Commands.Logout;
using Cases.Application.Identity.Queries.GetSession;
using Cases.Infrastructure.Authentication.Session;
using Cases.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cases.API.Controllers;

public sealed class AuthController : ApiControllerBase
{
    private readonly SessionSettings _sessionSettings;

    public AuthController(IOptions<SessionSettings> sessionOptions)
    {
        _sessionSettings = sessionOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("telegram")]
    public async Task<ActionResult<AuthResponse>> AuthenticateWithTelegram(
        [FromBody] TelegramAuthRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new AuthenticateTelegramCommand(request.InitData), cancellationToken);

        IssueSessionCookie(result.SessionId, result.SessionExpiresAt);

        var expiresIn = (long)Math.Max(0, (result.TokenExpiresAt - DateTimeOffset.UtcNow).TotalSeconds);

        var response = new AuthResponse(
            result.AccessToken,
            expiresIn,
            result.TokenExpiresAt,
            null,
            result.SessionExpiresAt,
            result.User);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("session")]
    public async Task<ActionResult<SessionResponse>> GetSession(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSessionQuery(), cancellationToken);

        IssueSessionCookie(result.SessionId, result.SessionExpiresAt);

        var response = new SessionResponse(result.User, result.SessionExpiresAt);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await Mediator.Send(new LogoutCommand(), cancellationToken);

        ClearSessionCookie();

        return NoContent();
    }

    private void IssueSessionCookie(Guid sessionId, DateTimeOffset expiresAt)
    {
        var cookieName = SessionCookieOptionsFactory.ResolveCookieName(_sessionSettings);
        var options = SessionCookieOptionsFactory.Create(_sessionSettings, expiresAt);
        Response.Cookies.Append(cookieName, sessionId.ToString(), options);
    }

    private void ClearSessionCookie()
    {
        var cookieName = SessionCookieOptionsFactory.ResolveCookieName(_sessionSettings);
        var options = SessionCookieOptionsFactory.CreateExpired(_sessionSettings);
        Response.Cookies.Append(cookieName, string.Empty, options);
    }
}

public sealed record TelegramAuthRequest(string InitData);

public sealed record AuthResponse(
    string Token,
    long ExpiresIn,
    DateTimeOffset ExpiresAt,
    string? RefreshToken,
    DateTimeOffset SessionExpiresAt,
    AuthenticatedUserDto User);

public sealed record SessionResponse(
    AuthenticatedUserDto User,
    DateTimeOffset SessionExpiresAt);
