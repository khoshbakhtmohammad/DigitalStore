using ProductService.Application.DTOs;

namespace ProductService.Application.Interfaces;

public interface IProductReadRepository
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
