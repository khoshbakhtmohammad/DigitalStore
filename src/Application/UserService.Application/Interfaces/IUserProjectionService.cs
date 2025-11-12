using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IUserProjectionService
{
    Task ProjectAsync(User user, CancellationToken cancellationToken = default);
}
