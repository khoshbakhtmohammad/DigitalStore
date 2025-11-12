using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Infrastructure.Persistence.Sql;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ProductReadDbContext _context;

    public ProductReadRepository(ProductReadDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _context.Products.ToListAsync(cancellationToken);

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });
    }
}
