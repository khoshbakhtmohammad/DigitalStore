namespace ProductService.Domain.Events;

public record ProductStockUpdatedEvent(Guid ProductId, int NewStockQuantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

