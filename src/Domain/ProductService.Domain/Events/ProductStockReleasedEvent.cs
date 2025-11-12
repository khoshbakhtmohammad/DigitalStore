namespace ProductService.Domain.Events;

public record ProductStockReleasedEvent(Guid ProductId, int ReleasedQuantity, int NewStockQuantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

