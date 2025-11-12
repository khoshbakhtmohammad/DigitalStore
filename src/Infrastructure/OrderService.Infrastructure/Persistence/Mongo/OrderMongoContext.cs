using MongoDB.Driver;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Mongo;

public class OrderMongoContext
{
    private readonly IMongoDatabase _database;

    public OrderMongoContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("orderItems");
}

