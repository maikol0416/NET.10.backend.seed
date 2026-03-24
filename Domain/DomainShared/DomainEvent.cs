namespace Domain.DomainShared;

public record DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
