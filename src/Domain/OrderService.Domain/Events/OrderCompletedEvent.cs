namespace OrderService.Domain.Events;

public record OrderCompletedEvent(Guid OrderId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

