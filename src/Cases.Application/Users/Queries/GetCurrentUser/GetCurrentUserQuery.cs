using MediatR;

namespace Cases.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IRequest<UserProfileDto>;
