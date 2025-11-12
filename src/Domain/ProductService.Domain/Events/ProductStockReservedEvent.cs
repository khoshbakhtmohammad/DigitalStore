namespace ProductService.Domain.Events;

public record ProductStockReservedEvent(Guid ProductId, int ReservedQuantity, int RemainingStock) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

