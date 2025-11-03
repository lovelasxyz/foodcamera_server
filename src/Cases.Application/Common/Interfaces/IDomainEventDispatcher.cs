using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Common;

namespace Cases.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}