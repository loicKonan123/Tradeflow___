namespace TradeFlow.Domain.Catalog;

public sealed record ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.NewGuid());
    public static ProductId From(Guid value) => new(value);
}
