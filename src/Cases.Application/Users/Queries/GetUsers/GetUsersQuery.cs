using MediatR;

namespace Cases.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int Limit = 10) : IRequest<IReadOnlyList<UserDto>>;
