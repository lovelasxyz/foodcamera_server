using System.Reflection;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Security;
using MediatR;

namespace Cases.Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Count == 0)
        {
            return await next();
        }

        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var requiredRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .SelectMany(a => a.Roles!.Split(','))
            .Select(r => r.Trim())
            .ToList();

        if (requiredRoles.Count > 0)
        {
            var hasRole = requiredRoles.Any(role => _currentUserService.Roles.Contains(role));
            if (!hasRole)
            {
                throw new UnauthorizedAccessException($"User does not have required roles: {string.Join(", ", requiredRoles)}");
            }
        }

        return await next();
    }
}
