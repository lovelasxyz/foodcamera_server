using Cases.Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[Authorize]
public sealed class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetUsers([FromQuery] int limit = 10)
    {
        var users = await Mediator.Send(new GetUsersQuery(limit));
        return Ok(users);
    }
}
