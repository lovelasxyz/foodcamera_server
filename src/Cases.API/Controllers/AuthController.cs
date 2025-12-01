using System;
using System.Threading;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Application.Identity.Commands.AuthenticateTelegram;
using Cases.Application.Identity.Commands.AuthenticateGuest;
using Cases.Application.Identity.Commands.Logout;
using Cases.Application.Identity.Queries.GetSession;
using Cases.Domain.Entities;
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
    private readonly IUserSessionService _userSessionService;
    private readonly ITokenService _tokenService;

    public AuthController(
        IOptions<SessionSettings> sessionOptions,
        IUserSessionService userSessionService,
        ITokenService tokenService)
    {
        _sessionSettings = sessionOptions.Value;
        _userSessionService = userSessionService;
        _tokenService = tokenService;
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

    [AllowAnonymous]
    [HttpPost("guest")]
    public async Task<ActionResult<AuthResponse>> AuthenticateGuest(CancellationToken cancellationToken)
    {
        var existingSession = await TryResumeExistingSessionAsync(cancellationToken);

        if (existingSession is not null)
        {
            IssueSessionCookie(existingSession.SessionId, existingSession.SessionExpiresAt);
            return Ok(CreateAuthResponse(existingSession));
        }

        var result = await Mediator.Send(new AuthenticateGuestCommand(), cancellationToken);

        IssueSessionCookie(result.SessionId, result.SessionExpiresAt);

        return Ok(CreateAuthResponse(result));
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

    private async Task<AuthenticationResult?> TryResumeExistingSessionAsync(CancellationToken cancellationToken)
    {
        var sessionId = ReadSessionIdFromCookie();
        if (sessionId is null)
        {
            return null;
        }

        var session = await _userSessionService.ValidateAsync(sessionId.Value, cancellationToken);
        if (session?.User is null)
        {
            return null;
        }

        var tokenResult = _tokenService.GenerateToken(session.User);
        var userDto = MapToDto(session.User);

        return new AuthenticationResult(
            tokenResult.Token,
            tokenResult.ExpiresAt,
            session.Id,
            session.ExpiresAt,
            userDto);
    }

    private Guid? ReadSessionIdFromCookie()
    {
        var cookieName = SessionCookieOptionsFactory.ResolveCookieName(_sessionSettings);
        if (!Request.Cookies.TryGetValue(cookieName, out var cookieValue))
        {
            return null;
        }

        return Guid.TryParse(cookieValue, out var sessionId) ? sessionId : null;
    }

    private static AuthenticatedUserDto MapToDto(User user)
    {
        return new AuthenticatedUserDto(
            user.Id,
            user.Name,
            user.Role.ToString(),
            user.TelegramUsername,
            user.Balance);
    }

    private static AuthResponse CreateAuthResponse(AuthenticationResult result)
    {
        var expiresIn = CalculateExpiresIn(result.TokenExpiresAt);

        return new AuthResponse(
            result.AccessToken,
            expiresIn,
            result.TokenExpiresAt,
            null,
            result.SessionExpiresAt,
            result.User);
    }

    private static long CalculateExpiresIn(DateTimeOffset expiresAt)
    {
        return (long)Math.Max(0, (expiresAt - DateTimeOffset.UtcNow).TotalSeconds);
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
