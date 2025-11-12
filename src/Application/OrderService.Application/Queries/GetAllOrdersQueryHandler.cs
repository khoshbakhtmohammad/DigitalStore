using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderReadRepository _readRepository;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(
        IOrderReadRepository readRepository,
        ILogger<GetAllOrdersQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all orders");
        return await _readRepository.GetAllAsync(cancellationToken);
    }
}

