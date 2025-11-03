using Cases.Application.Common.Models;
using MediatR;

namespace Cases.Application.Identity.Queries.GetSession;

public sealed record GetSessionQuery() : IRequest<SessionStateResult>;
