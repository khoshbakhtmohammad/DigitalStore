namespace OrderService.Domain.ValueObjects;

public record Money(decimal Value, string Currency = "USD")
{
    public static Money Zero => new(0);
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(left.Value + right.Value, left.Currency);
    }
    
    public static Money operator *(Money money, int multiplier)
    {
        return new Money(money.Value * multiplier, money.Currency);
    }
}

