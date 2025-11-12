using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductReadRepository _readRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductReadRepository readRepository,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", request.ProductId);
            return null;
        }

        return product;
    }
}

