using Cases.Application.Common.Models;
using MediatR;

namespace Cases.Application.Identity.Commands.AuthenticateTelegram;

public sealed record AuthenticateTelegramCommand(string InitData) : IRequest<AuthenticationResult>;
