using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Orders;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");

        builder.Property(i => i.OrderId)
            .HasColumnName("order_id")
            .HasConversion(id => id.Value, value => OrderId.From(value));

        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .HasConversion(id => id.Value, value => ProductId.From(value));

        builder.Property(i => i.ProductTitle).HasColumnName("product_title").HasMaxLength(200).IsRequired();
        builder.Property(i => i.Quantity).HasColumnName("quantity").HasDefaultValue(1);
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");

        builder.OwnsOne(i => i.UnitPrice, m =>
        {
            m.Property(x => x.Amount).HasColumnName("unit_price").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("unit_currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });
        builder.OwnsOne(i => i.LineTotal, m =>
        {
            m.Property(x => x.Amount).HasColumnName("line_total").HasColumnType("numeric(12,2)");
            m.Property(x => x.Currency).HasColumnName("line_currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => new Currency(code));
        });
    }
}
