using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence.Sql;

namespace OrderService.Infrastructure.Projections;

public class OrderProjectionService : IOrderProjectionService
{
    private readonly OrderReadDbContext _context;
    private readonly ILogger<OrderProjectionService> _logger;

    public OrderProjectionService(
        OrderReadDbContext context,
        ILogger<OrderProjectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        if (existing != null)
        {
            _context.OrderItems.RemoveRange(existing.Items);
            _context.Orders.Remove(existing);
        }

        var readModel = BuildReadModel(order);
        await _context.Orders.AddAsync(readModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Projected order {OrderId} to read model", order.Id);
    }

    private static OrderReadModel BuildReadModel(Order order)
    {
        return new OrderReadModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount.Value,
            Currency = order.TotalAmount.Currency,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(item => new OrderItemReadModel
            {
                Id = item.Id,
                OrderId = order.Id,
                ProductId = item.ProductId.Value,
                ProductName = item.ProductName,
                Price = item.Price.Value,
                Currency = item.Price.Currency,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}
