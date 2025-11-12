using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenSleigh.Transport;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using Shared;
using System.Net;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IMediator mediator, 
        IMessageBus messageBus,
        ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            idempotencyKey
        );

        var result = await _mediator.Send(command, cancellationToken);
        
        var startOrderSaga = new StartOrderSaga(
            result.Id,
            result.CustomerId,
            result.TotalAmount
        );
        await _messageBus.PublishAsync(startOrderSaga, cancellationToken);
        
        _logger.LogInformation("Order {OrderId} created and saga triggered", result.Id);
        
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrdersByCustomerQuery(customerId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllOrdersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<OrderDto>> ChangeOrderStatus(
        Guid id,
        [FromBody] ChangeOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ChangeOrderStatusCommand(
            id,
            request.Status,
            request.Reason
        );

        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to change order {OrderId} status: {Error}", id, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record CreateOrderRequest(
    Guid CustomerId,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity
);

public record ChangeOrderStatusRequest(
    OrderService.Domain.Entities.OrderStatus Status,
    string? Reason = null
);

