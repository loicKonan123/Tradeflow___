namespace TradeFlow.Domain.Catalog;

public sealed record ProductMediaId(Guid Value)
{
    public static ProductMediaId New() => new(Guid.NewGuid());
    public static ProductMediaId From(Guid value) => new(value);
}
