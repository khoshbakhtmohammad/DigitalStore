namespace OrderService.Domain.ValueObjects;

public record CustomerId(Guid Value)
{
    public static CustomerId From(Guid value) => new(value);
    public static CustomerId New() => new(Guid.NewGuid());
}

