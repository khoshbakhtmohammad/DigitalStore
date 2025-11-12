namespace OrderService.Domain.Events;

public record OrderFailedEvent(Guid OrderId, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

