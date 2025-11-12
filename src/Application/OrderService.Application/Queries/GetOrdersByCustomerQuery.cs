using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<OrderDto>>;

