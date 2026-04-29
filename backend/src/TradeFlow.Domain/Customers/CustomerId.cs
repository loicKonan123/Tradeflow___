namespace TradeFlow.Domain.Customers;

public sealed record CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId From(Guid value) => new(value);
}
