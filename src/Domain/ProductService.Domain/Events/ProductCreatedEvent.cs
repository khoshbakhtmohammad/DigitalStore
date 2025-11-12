namespace ProductService.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

