using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto?>;

