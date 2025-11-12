using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderProjectionService
{
    Task ProjectAsync(Order order, CancellationToken cancellationToken = default);
}
