using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Commands;

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProjectionService _projectionService;
    private readonly ILogger<ChangeOrderStatusCommandHandler> _logger;

    public ChangeOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IOrderProjectionService projectionService,
        ILogger<ChangeOrderStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _projectionService = projectionService;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            throw new InvalidOperationException($"Order with id {request.OrderId} not found");
        }

        var oldStatus = order.Status;

        switch (request.NewStatus)
        {
            case OrderStatus.Processing:
                if (order.Status != OrderStatus.Pending)
                {
                    throw new InvalidOperationException($"Cannot change order status to Processing. Current status: {order.Status}");
                }
                order.MarkAsProcessing();
                break;

            case OrderStatus.Completed:
                if (order.Status != OrderStatus.Processing)
                {
                    throw new InvalidOperationException($"Cannot change order status to Completed. Current status: {order.Status}");
                }
                order.MarkAsCompleted();
                break;

            case OrderStatus.Failed:
                order.MarkAsFailed(request.Reason ?? "Order failed");
                break;

            case OrderStatus.Cancelled:
                order.MarkAsCancelled(request.Reason);
                break;

            default:
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {request.NewStatus}");
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _projectionService.ProjectAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} status changed from {OldStatus} to {NewStatus}", 
            order.Id, oldStatus, order.Status);

        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount.Value,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
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

