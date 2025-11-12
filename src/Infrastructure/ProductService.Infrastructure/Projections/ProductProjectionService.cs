using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence.Sql;

namespace ProductService.Infrastructure.Projections;

public class ProductProjectionService : IProductProjectionService
{
    private readonly ProductReadDbContext _context;
    private readonly ILogger<ProductProjectionService> _logger;

    public ProductProjectionService(
        ProductReadDbContext context,
        ILogger<ProductProjectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProjectAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);
        if (existing != null)
        {
            _context.Products.Remove(existing);
        }

        var readModel = new ProductReadModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Value,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        await _context.Products.AddAsync(readModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Projected product {ProductId} to read model", product.Id);
    }
}
