using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries;

public record GetAllProductsQuery() : IRequest<IEnumerable<ProductDto>>;

