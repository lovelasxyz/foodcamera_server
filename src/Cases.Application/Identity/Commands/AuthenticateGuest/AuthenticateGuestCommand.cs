using Cases.Application.Common.Models;
using MediatR;

namespace Cases.Application.Identity.Commands.AuthenticateGuest;

public sealed record AuthenticateGuestCommand() : IRequest<AuthenticationResult>;
