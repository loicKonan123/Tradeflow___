using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class CustomProjectRequestConfiguration : IEntityTypeConfiguration<CustomProjectRequest>
{
    public void Configure(EntityTypeBuilder<CustomProjectRequest> builder)
    {
        builder.ToTable("custom_project_requests");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, v => CustomProjectRequestId.From(v))
            .HasColumnName("id");
        builder.Property(p => p.CustomerId)
            .HasConversion(id => id.Value, v => CustomerId.From(v))
            .HasColumnName("customer_id");
        builder.Property(p => p.Market).IsRequired().HasMaxLength(100).HasColumnName("market");
        builder.Property(p => p.Timeframe).IsRequired().HasMaxLength(50).HasColumnName("timeframe");
        builder.Property(p => p.EntryConditions).IsRequired().HasColumnName("entry_conditions");
        builder.Property(p => p.ExitConditions).IsRequired().HasColumnName("exit_conditions");
        builder.Property(p => p.RiskManagement).IsRequired().HasColumnName("risk_management");
        builder.Property(p => p.AdditionalNotes).HasColumnName("additional_notes");
        builder.Property(p => p.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(p => p.QuotedPrice).HasColumnType("decimal(18,2)").HasColumnName("quoted_price");
        builder.Property(p => p.QuotedCurrency).HasMaxLength(3).HasColumnName("quoted_currency");
        builder.Property(p => p.DepositAmount).HasColumnType("decimal(18,2)").HasColumnName("deposit_amount");
        builder.Property(p => p.AdminNotes).HasColumnName("admin_notes");
        builder.Property(p => p.DeliveryFileUrl).HasMaxLength(500).HasColumnName("delivery_file_url");
        builder.Property(p => p.DeliveryNotes).HasColumnName("delivery_notes");
        builder.Property(p => p.QuotedAt).HasColumnName("quoted_at");
        builder.Property(p => p.DepositPaidAt).HasColumnName("deposit_paid_at");
        builder.Property(p => p.DeliveredAt).HasColumnName("delivered_at");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Ignore(p => p.DomainEvents);
    }
}
