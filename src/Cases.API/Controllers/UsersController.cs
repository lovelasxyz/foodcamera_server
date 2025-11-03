using Cases.Application.Common.Models;
using Cases.Application.Users.Queries.GetCurrentUser;
using Cases.Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[Authorize]
public sealed class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await Mediator.Send(new GetUsersQuery(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var profile = await Mediator.Send(new GetCurrentUserQuery());
        return Ok(profile);
    }
}
