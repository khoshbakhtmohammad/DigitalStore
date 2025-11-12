namespace UserService.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

