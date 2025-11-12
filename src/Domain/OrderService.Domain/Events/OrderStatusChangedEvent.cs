using OrderService.Domain.Entities;

namespace OrderService.Domain.Events;

public record OrderStatusChangedEvent(Guid OrderId, OrderStatus OldStatus, OrderStatus NewStatus) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

