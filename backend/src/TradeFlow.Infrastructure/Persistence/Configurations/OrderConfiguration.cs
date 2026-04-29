using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, v => OrderId.From(v))
            .HasColumnName("id");
        builder.Property(o => o.CustomerId)
            .HasConversion(id => id.Value, v => CustomerId.From(v))
            .HasColumnName("customer_id");
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50).HasColumnName("order_number");
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.Total).HasColumnType("decimal(18,2)").HasColumnName("total");
        builder.Property(o => o.Currency).HasMaxLength(3).HasColumnName("currency");
        builder.Property(o => o.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(o => o.StripePaymentIntentId).HasMaxLength(200).HasColumnName("stripe_payment_intent_id");
        builder.Property(o => o.StripeSessionId).HasMaxLength(200).HasColumnName("stripe_session_id");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");
        builder.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
        builder.Ignore(o => o.DomainEvents);
    }
}
