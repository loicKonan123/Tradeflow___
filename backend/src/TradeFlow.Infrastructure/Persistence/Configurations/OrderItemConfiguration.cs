using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, v => OrderItemId.From(v))
            .HasColumnName("id");
        builder.Property(i => i.OrderId)
            .HasConversion(id => id.Value, v => OrderId.From(v))
            .HasColumnName("order_id");
        builder.Property(i => i.ProductId)
            .HasConversion(id => id.Value, v => ProductId.From(v))
            .HasColumnName("product_id");
        builder.Property(i => i.ProductTitle).IsRequired().HasMaxLength(200).HasColumnName("product_title");
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").HasColumnName("unit_price");
        builder.Property(i => i.Currency).HasMaxLength(3).HasColumnName("currency");
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
    }
}
