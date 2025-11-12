using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(
        IOrderReadRepository readRepository,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _readRepository.GetByIdAsync(request.OrderId, cancellationToken);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return null;
        }

        return order;
    }
}

