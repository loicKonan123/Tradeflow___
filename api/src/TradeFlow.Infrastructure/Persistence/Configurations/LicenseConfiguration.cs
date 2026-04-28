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
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => LicenseId.From(value));

        builder.Property(l => l.CustomerId)
            .HasColumnName("customer_id")
            .HasConversion(id => id.Value, value => CustomerId.From(value));

        builder.Property(l => l.ProductId)
            .HasColumnName("product_id")
            .HasConversion(id => id.Value, value => ProductId.From(value));

        builder.Property(l => l.SourceOrderId)
            .HasColumnName("source_order_id")
            .HasConversion(id => id.Value, value => OrderId.From(value));

        builder.Property(l => l.GrantedToTvUsername).HasColumnName("granted_to_tv_username").HasMaxLength(30).IsRequired();
        builder.Property(l => l.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(l => l.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(l => l.IssuedAt).HasColumnName("issued_at");
        builder.Property(l => l.ExpiresAt).HasColumnName("expires_at");
        builder.Property(l => l.AccessGrantedAt).HasColumnName("access_granted_at");
        builder.Property(l => l.RevokedAt).HasColumnName("revoked_at");
        builder.Property(l => l.RevocationReason).HasColumnName("revocation_reason");
        builder.Property(l => l.CreatedAt).HasColumnName("created_at");
        builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(l => l.CustomerId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => new { l.CustomerId, l.ProductId });
    }
}
