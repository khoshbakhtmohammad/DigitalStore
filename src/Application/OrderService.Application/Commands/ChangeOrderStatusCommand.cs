using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands;

public record ChangeOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? Reason = null
) : IRequest<OrderDto>;

