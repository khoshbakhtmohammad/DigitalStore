using ProductService.Domain.Events;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int StockQuantity { get; private set; }
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { }

    public Product(string name, string description, Money price, int stockQuantity)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Price = price;
        StockQuantity = stockQuantity >= 0 ? stockQuantity : throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
        Status = ProductStatus.Active;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price.Value));
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Value <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductPriceUpdatedEvent(Id, oldPrice.Value, newPrice.Value));
    }

    public void UpdateStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new InvalidOperationException("Insufficient stock");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockUpdatedEvent(Id, StockQuantity));
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockReservedEvent(Id, quantity, StockQuantity));
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockReleasedEvent(Id, quantity, StockQuantity));
    }

    public void Deactivate()
    {
        if (Status == ProductStatus.Inactive)
            return;

        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (Status == ProductStatus.Active)
            return;

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductActivatedEvent(Id));
    }
}

public enum ProductStatus
{
    Active,
    Inactive,
    Discontinued
}

