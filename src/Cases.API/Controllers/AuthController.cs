using Cases.Application.Common.Models;
using Cases.Application.Identity.Commands.AuthenticateTelegram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[AllowAnonymous]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("telegram")]
    public async Task<ActionResult<AuthenticationResult>> AuthenticateWithTelegram([FromBody] TelegramAuthRequest request)
    {
        var result = await Mediator.Send(new AuthenticateTelegramCommand(request.InitData));
        return Ok(result);
    }
}

public sealed record TelegramAuthRequest(string InitData);
