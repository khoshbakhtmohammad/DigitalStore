using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence.Sql;

namespace UserService.Infrastructure.Projections;

public class UserProjectionService : IUserProjectionService
{
    private readonly UserReadDbContext _context;
    private readonly ILogger<UserProjectionService> _logger;

    public UserProjectionService(
        UserReadDbContext context,
        ILogger<UserProjectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProjectAsync(User user, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (existing != null)
        {
            _context.Users.Remove(existing);
        }

        var readModel = new UserReadModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        await _context.Users.AddAsync(readModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Projected user {UserId} to read model", user.Id);
    }
}
