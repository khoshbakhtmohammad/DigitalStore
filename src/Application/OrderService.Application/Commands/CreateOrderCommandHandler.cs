using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIdempotencyService _idempotencyService;
    private readonly IOrderProjectionService _projectionService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IIdempotencyService idempotencyService,
        IOrderProjectionService projectionService,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _idempotencyService = idempotencyService;
        _projectionService = projectionService;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var existingOrder = await _idempotencyService.GetOrderByKeyAsync(request.IdempotencyKey, cancellationToken);
            if (existingOrder != null)
            {
                _logger.LogInformation("Idempotent request detected. Returning existing order {OrderId}", existingOrder.Id);
                return existingOrder;
            }
        }

        var customerId = CustomerId.From(request.CustomerId);
        var orderItems = request.Items.Select(item => 
            new OrderItem(
                ProductId.From(item.ProductId),
                item.ProductName,
                new Money(item.Price),
                item.Quantity
            )).ToList();

        var order = new Order(customerId, orderItems);

        await _orderRepository.AddAsync(order, cancellationToken);

        await _projectionService.ProjectAsync(order, cancellationToken);
        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            await _idempotencyService.StoreIdempotencyKeyAsync(request.IdempotencyKey, order.Id, cancellationToken);
        }

        _logger.LogInformation("Order {OrderId} created successfully and projected to read model", order.Id);

        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount.Value,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId.Value,
                ProductName = i.ProductName,
                Price = i.Price.Value,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}

