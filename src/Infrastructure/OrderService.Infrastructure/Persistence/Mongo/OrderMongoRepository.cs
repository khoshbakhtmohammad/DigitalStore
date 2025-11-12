using MongoDB.Driver;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence.Mongo;

namespace OrderService.Infrastructure.Persistence.Mongo;

public class OrderMongoRepository : IOrderRepository
{
    private readonly OrderMongoContext _context;

    public OrderMongoRepository(OrderMongoContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        return await _context.Orders.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.CustomerId.Value, customerId);
        return await _context.Orders.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.InsertOneAsync(order, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);
        await _context.Orders.ReplaceOneAsync(filter, order, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var count = await _context.Orders.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}

