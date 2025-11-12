namespace UserService.Domain.Events;

public record UserDeactivatedEvent(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

