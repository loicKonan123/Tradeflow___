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
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => CustomerId.From(value));

        builder.Property(c => c.FirebaseUid).HasColumnName("firebase_uid").HasMaxLength(128).IsRequired();
        builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(c => c.DisplayName).HasColumnName("display_name").HasMaxLength(100);
        builder.Property(c => c.CountryCode).HasColumnName("country_code").HasMaxLength(2);
        builder.Property(c => c.PreferredCurrency).HasColumnName("preferred_currency").HasMaxLength(3).HasDefaultValue("EUR");
        builder.Property(c => c.TradingViewUsername).HasColumnName("tradingview_username").HasMaxLength(30);
        builder.Property(c => c.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(c => c.FirebaseUid).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
