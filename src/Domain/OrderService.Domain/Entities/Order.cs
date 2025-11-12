using OrderService.Domain.ValueObjects;
using OrderService.Domain.Events;

namespace OrderService.Domain.Entities;

public class Order : Entity<Guid>
{
    public OrderStatus Status { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(CustomerId customerId, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        _items = items ?? throw new ArgumentNullException(nameof(items));
        
        CalculateTotal();
        
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId.Value, TotalAmount.Value));
    }

    public void MarkAsProcessing()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot mark order as processing. Current status: {Status}");
        
        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(Id, OrderStatus.Pending, OrderStatus.Processing));
    }

    public void MarkAsCompleted()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Cannot mark order as completed. Current status: {Status}");
        
        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCompletedEvent(Id));
    }

    public void MarkAsFailed(string reason)
    {
        Status = OrderStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderFailedEvent(Id, reason));
    }

    public void MarkAsCancelled(string? reason = null)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed order");
        
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderFailedEvent(Id, reason ?? "Order cancelled"));
    }

    private void CalculateTotal()
    {
        TotalAmount = new Money(_items.Sum(item => item.Price.Value * item.Quantity));
    }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}

