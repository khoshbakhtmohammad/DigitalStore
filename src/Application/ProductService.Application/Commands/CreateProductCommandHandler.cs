using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductProjectionService _projectionService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductProjectionService projectionService,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _projectionService = projectionService;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Name,
            request.Description,
            new Money(request.Price),
            request.StockQuantity
        );

        await _productRepository.AddAsync(product, cancellationToken);

        await _projectionService.ProjectAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} created successfully and projected to read model", product.Id);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Value,
            StockQuantity = product.StockQuantity,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt
        };
    }
}

