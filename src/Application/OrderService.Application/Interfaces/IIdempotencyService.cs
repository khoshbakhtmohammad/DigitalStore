using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces;

public interface IIdempotencyService
{
    Task<OrderDto?> GetOrderByKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task StoreIdempotencyKeyAsync(string idempotencyKey, Guid orderId, CancellationToken cancellationToken = default);
}

