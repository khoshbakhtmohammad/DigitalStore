using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetOrdersByCustomerQueryHandler> _logger;

    public GetOrdersByCustomerQueryHandler(
        IOrderReadRepository readRepository,
        ILogger<GetOrdersByCustomerQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _readRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
    }
}

