using Cases.Application.Game.Commands.Spin;
using Cases.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[Authorize]
public sealed class GameController : ApiControllerBase
{
    [HttpPost("spin")]
    public async Task<ActionResult<SpinResultDto>> Spin(
        [FromBody] SpinRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SpinCommand(request.CaseId, request.RequestId);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

public record SpinRequest(string CaseId, string RequestId);
