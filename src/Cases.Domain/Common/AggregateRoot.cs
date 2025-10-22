namespace Cases.Domain.Common;

public abstract class AggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot
{
}
