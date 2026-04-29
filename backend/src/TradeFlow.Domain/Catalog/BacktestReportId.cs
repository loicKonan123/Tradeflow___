namespace TradeFlow.Domain.Catalog;

public sealed record BacktestReportId(Guid Value)
{
    public static BacktestReportId New() => new(Guid.NewGuid());
    public static BacktestReportId From(Guid value) => new(value);
}
