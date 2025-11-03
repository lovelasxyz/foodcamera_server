using Cases.Domain.Common;
using MediatR;

namespace Cases.Application.Common.Models;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;