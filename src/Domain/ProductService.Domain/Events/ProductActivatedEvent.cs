namespace ProductService.Domain.Events;

public record ProductActivatedEvent(Guid ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

