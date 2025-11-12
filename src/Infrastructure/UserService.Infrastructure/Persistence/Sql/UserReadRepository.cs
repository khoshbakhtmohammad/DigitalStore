using Microsoft.EntityFrameworkCore;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Persistence.Sql;

public class UserReadRepository : IUserReadRepository
{
    private readonly UserReadDbContext _context;

    public UserReadRepository(UserReadDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users.ToListAsync(cancellationToken);

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }
}
