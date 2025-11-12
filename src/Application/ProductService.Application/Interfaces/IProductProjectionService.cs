using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces;

public interface IProductProjectionService
{
    Task ProjectAsync(Product product, CancellationToken cancellationToken = default);
}
