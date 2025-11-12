using MongoDB.Driver;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence.Mongo;

namespace ProductService.Infrastructure.Persistence.Mongo;

public class ProductMongoRepository : IProductRepository
{
    private readonly ProductMongoContext _context;

    public ProductMongoRepository(ProductMongoContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        return await _context.Products.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Status, ProductStatus.Active);
        return await _context.Products.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.InsertOneAsync(product, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        await _context.Products.ReplaceOneAsync(filter, product, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var count = await _context.Products.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}

