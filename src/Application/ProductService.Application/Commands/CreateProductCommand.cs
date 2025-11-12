using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<ProductDto>;

