namespace Cases.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
