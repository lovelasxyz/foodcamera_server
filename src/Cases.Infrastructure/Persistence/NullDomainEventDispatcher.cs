using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Common;

namespace Cases.Infrastructure.Persistence;

internal sealed class NullDomainEventDispatcher : IDomainEventDispatcher
{
    private NullDomainEventDispatcher()
    {
    }

    public static NullDomainEventDispatcher Instance { get; } = new();

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}