namespace UserService.Domain.Events;

public record UserEmailUpdatedEvent(Guid UserId, string NewEmail) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

