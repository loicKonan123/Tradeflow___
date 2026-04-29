using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Licensing;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("licenses");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, v => LicenseId.From(v))
            .HasColumnName("id");
        builder.Property(l => l.CustomerId)
            .HasConversion(id => id.Value, v => CustomerId.From(v))
            .HasColumnName("customer_id");
        builder.Property(l => l.ProductId)
            .HasConversion(id => id.Value, v => ProductId.From(v))
            .HasColumnName("product_id");
        builder.Property(l => l.OrderId)
            .HasConversion(id => id.Value, v => OrderId.From(v))
            .HasColumnName("order_id");
        builder.Property(l => l.DownloadToken).IsRequired().HasMaxLength(64).HasColumnName("download_token");
        builder.HasIndex(l => l.DownloadToken).IsUnique();
        builder.Property(l => l.DownloadCount).HasColumnName("download_count");
        builder.Property(l => l.MaxDownloads).HasColumnName("max_downloads");
        builder.Property(l => l.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(l => l.CreatedAt).HasColumnName("created_at");
        builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");
        builder.Ignore(l => l.DomainEvents);
    }
}
