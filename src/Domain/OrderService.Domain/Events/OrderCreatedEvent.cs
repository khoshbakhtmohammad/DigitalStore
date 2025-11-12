namespace OrderService.Domain.Events;

public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

