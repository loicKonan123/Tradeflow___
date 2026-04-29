using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.ToTable("product_medias");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, v => ProductMediaId.From(v))
            .HasColumnName("id");
        builder.Property(m => m.ProductId)
            .HasConversion(id => id.Value, v => ProductId.From(v))
            .HasColumnName("product_id");
        builder.Property(m => m.Url).IsRequired().HasMaxLength(500).HasColumnName("url");
        builder.Property(m => m.Type).HasConversion<string>().HasColumnName("type");
        builder.Property(m => m.SortOrder).HasColumnName("sort_order");
        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
    }
}
