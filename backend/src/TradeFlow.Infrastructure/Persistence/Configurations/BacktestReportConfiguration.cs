using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class BacktestReportConfiguration : IEntityTypeConfiguration<BacktestReport>
{
    public void Configure(EntityTypeBuilder<BacktestReport> builder)
    {
        builder.ToTable("backtest_reports");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasConversion(id => id.Value, v => BacktestReportId.From(v))
            .HasColumnName("id");
        builder.Property(b => b.ProductId)
            .HasConversion(id => id.Value, v => ProductId.From(v))
            .HasColumnName("product_id");
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
        builder.Property(b => b.WinRate).HasColumnType("decimal(5,2)").HasColumnName("win_rate");
        builder.Property(b => b.MaxDrawdown).HasColumnType("decimal(5,2)").HasColumnName("max_drawdown");
        builder.Property(b => b.ProfitFactor).HasColumnType("decimal(8,2)").HasColumnName("profit_factor");
        builder.Property(b => b.PeriodStart).HasColumnName("period_start");
        builder.Property(b => b.PeriodEnd).HasColumnName("period_end");
        builder.Property(b => b.Markets).HasMaxLength(500).HasColumnName("markets");
        builder.Property(b => b.Notes).HasColumnName("notes");
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");
    }
}
