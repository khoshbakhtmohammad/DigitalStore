namespace OrderService.Domain.ValueObjects;

public record ProductId(Guid Value)
{
    public static ProductId From(Guid value) => new(value);
    public static ProductId New() => new(Guid.NewGuid());
}

