using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces;

public interface IOrderReadRepository
{
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
