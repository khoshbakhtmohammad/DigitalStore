namespace ProductService.Domain.Events;

public record ProductDeactivatedEvent(Guid ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

