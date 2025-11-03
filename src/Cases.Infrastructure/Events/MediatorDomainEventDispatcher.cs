using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Models;
using Cases.Domain.Common;
using MediatR;

namespace Cases.Infrastructure.Events;

public sealed class MediatorDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatorDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notification = CreateNotification(domainEvent);
            await _publisher.Publish(notification, cancellationToken).ConfigureAwait(false);
        }
    }

    private static INotification CreateNotification(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}