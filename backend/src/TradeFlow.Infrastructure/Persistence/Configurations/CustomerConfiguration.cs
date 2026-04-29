using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, v => CustomerId.From(v))
            .HasColumnName("id");
        builder.Property(c => c.FirebaseUid).IsRequired().HasMaxLength(128).HasColumnName("firebase_uid");
        builder.HasIndex(c => c.FirebaseUid).IsUnique();
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256).HasColumnName("email");
        builder.Property(c => c.DisplayName).HasMaxLength(100).HasColumnName("display_name");
        builder.Property(c => c.TradingViewUsername).HasMaxLength(100).HasColumnName("tradingview_username");
        builder.Property(c => c.IsAdmin).HasColumnName("is_admin");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
