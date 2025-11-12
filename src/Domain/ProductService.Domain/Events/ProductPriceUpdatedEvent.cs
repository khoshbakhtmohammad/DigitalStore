namespace ProductService.Domain.Events;

public record ProductPriceUpdatedEvent(Guid ProductId, decimal OldPrice, decimal NewPrice) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

