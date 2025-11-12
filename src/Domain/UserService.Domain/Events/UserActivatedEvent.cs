namespace UserService.Domain.Events;

public record UserActivatedEvent(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

