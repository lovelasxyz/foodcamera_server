using MediatR;

namespace Cases.Application.Identity.Commands.Logout;

public sealed record LogoutCommand() : IRequest;
