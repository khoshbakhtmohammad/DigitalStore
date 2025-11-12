namespace UserService.Domain.Events;

public record UserUpdatedEvent(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

