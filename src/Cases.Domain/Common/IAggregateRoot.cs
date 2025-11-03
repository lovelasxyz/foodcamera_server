using System.Collections.Generic;

namespace Cases.Domain.Common;

public interface IAggregateRoot
{
	IReadOnlyList<IDomainEvent> DomainEvents { get; }
	void ClearDomainEvents();
}
