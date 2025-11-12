using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IUserReadRepository
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
