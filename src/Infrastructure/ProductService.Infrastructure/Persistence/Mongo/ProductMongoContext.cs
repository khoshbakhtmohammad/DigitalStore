using MongoDB.Driver;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Mongo;

public class ProductMongoContext
{
    private readonly IMongoDatabase _database;

    public ProductMongoContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
}

