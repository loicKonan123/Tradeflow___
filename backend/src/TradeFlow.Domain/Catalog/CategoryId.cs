namespace TradeFlow.Domain.Catalog;

public sealed record CategoryId(Guid Value)
{
    public static CategoryId New() => new(Guid.NewGuid());
    public static CategoryId From(Guid value) => new(value);
}
