using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Models;
using Cases.Application.Users.Interfaces;
using Cases.Domain.Enums;
using MediatR;

namespace Cases.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserReadRepository _users;

    public GetUsersQueryHandler(IUserReadRepository users)
    {
        _users = users;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _users.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Balance,
                user.Role.ToDatabaseValue(),
                user.TelegramUsername,
                user.LastAuthAt))
            .ToList();

        return new PagedResult<UserDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
