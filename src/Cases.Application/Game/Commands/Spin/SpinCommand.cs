using Cases.Application.Common.Models;
using MediatR;

namespace Cases.Application.Game.Commands.Spin;

public sealed record SpinCommand(string CaseId, string RequestId) : IRequest<SpinResultDto>;
