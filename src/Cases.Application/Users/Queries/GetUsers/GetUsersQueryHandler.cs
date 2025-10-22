using Cases.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cases.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetUsersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(user => user.CreatedAt)
            .Take(Math.Max(1, request.Limit))
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Balance,
                user.Role,
                user.TelegramUsername,
                user.LastAuthAt))
            .ToListAsync(cancellationToken);
    }
}
