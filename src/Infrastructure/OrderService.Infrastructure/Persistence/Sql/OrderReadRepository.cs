using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Persistence.Sql;

public class OrderReadRepository : IOrderReadRepository
{
    private readonly OrderReadDbContext _context;

    public OrderReadRepository(OrderReadDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        });
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ToListAsync(cancellationToken);

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        });
    }
}
