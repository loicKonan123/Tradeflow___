using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Orders;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => OrderId.From(value));

        builder.Property(o => o.OrderNumber).HasColumnName("order_number").HasMaxLength(20).IsRequired();
        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.CustomerId)
            .HasColumnName("customer_id")
            .HasConversion(id => id.Value, value => CustomerId.From(value));

        builder.Property(o => o.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(o => o.PaymentStatus).HasColumnName("payment_status").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(o => o.TradingViewUsername).HasColumnName("tradingview_username").HasMaxLength(30).IsRequired();
        builder.Property(o => o.StripePaymentIntentId).HasColumnName("stripe_payment_intent_id").HasMaxLength(100);
        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");
        builder.Property(o => o.PaidAt).HasColumnName("paid_at");
        builder.Property(o => o.FulfilledAt).HasColumnName("fulfilled_at");

        builder.OwnsOne(o => o.Subtotal, m =>
        {
            m.Property(x => x.Amount).HasColumnName("subtotal_amount").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });
        builder.OwnsOne(o => o.TaxAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("tax_amount").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("tax_currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });
        builder.OwnsOne(o => o.DiscountAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("discount_amount").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("discount_currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });
        builder.OwnsOne(o => o.Total, m =>
        {
            m.Property(x => x.Amount).HasColumnName("total_amount").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("total_currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("order_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}
