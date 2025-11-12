using UserService.Domain.Events;

namespace UserService.Domain.Entities;

public class User : Entity<Guid>
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { }

    public User(string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        AddDomainEvent(new UserCreatedEvent(Id, Email));
    }

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserUpdatedEvent(Id));
    }

    public void UpdateEmail(string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        Email = email;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserEmailUpdatedEvent(Id, email));
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
            return;

        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            return;

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}

