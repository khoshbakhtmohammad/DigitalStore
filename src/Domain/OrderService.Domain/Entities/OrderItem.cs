using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public class OrderItem : Entity<Guid>
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money Price { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(ProductId productId, string productName, Money price, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Price = price;
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
    }
}

