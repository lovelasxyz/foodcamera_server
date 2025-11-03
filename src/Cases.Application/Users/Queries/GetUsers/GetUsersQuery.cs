using Cases.Application.Common.Models;
using MediatR;

namespace Cases.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<UserDto>>;
