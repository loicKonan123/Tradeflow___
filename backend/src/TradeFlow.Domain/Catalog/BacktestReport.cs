using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Catalog;

public class BacktestReport : BaseEntity<BacktestReportId>
{
    public ProductId ProductId { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public decimal WinRate { get; private set; }
    public decimal MaxDrawdown { get; private set; }
    public decimal ProfitFactor { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public string Markets { get; private set; } = default!;
    public string? Notes { get; private set; }

    private BacktestReport() { }

    public static BacktestReport Create(
        ProductId productId,
        string title,
        decimal winRate,
        decimal maxDrawdown,
        decimal profitFactor,
        DateTime periodStart,
        DateTime periodEnd,
        string markets,
        string? notes = null)
        => new()
        {
            Id = BacktestReportId.New(),
            ProductId = productId,
            Title = title,
            WinRate = winRate,
            MaxDrawdown = maxDrawdown,
            ProfitFactor = profitFactor,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Markets = markets,
            Notes = notes
        };
}
