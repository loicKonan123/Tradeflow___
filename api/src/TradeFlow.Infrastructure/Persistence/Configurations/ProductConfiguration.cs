using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => ProductId.From(value));

        builder.Property(p => p.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(p => p.ShortDescription).HasColumnName("short_description").HasMaxLength(500);
        builder.Property(p => p.LongDescriptionMarkdown).HasColumnName("long_description_md");
        builder.Property(p => p.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).HasDefaultValue(ProductStatus.Draft);
        builder.Property(p => p.SalesCount).HasColumnName("sales_count").HasDefaultValue(0);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.PublishedAt).HasColumnName("published_at");

        builder.OwnsOne(p => p.Slug, slug =>
        {
            slug.Property(s => s.Value).HasColumnName("slug").HasMaxLength(150).IsRequired();
            slug.HasIndex(s => s.Value).IsUnique().HasDatabaseName("ix_products_slug");
        });

        builder.OwnsOne(p => p.BasePrice, price =>
        {
            price.Property(m => m.Amount).HasColumnName("base_price_amount").HasColumnType("numeric(12,2)").IsRequired();
            price.Property(m => m.Currency)
                .HasColumnName("base_price_currency").HasMaxLength(3).IsRequired()
                .HasConversion(c => c.Code, code => new Currency(code));
        });

        builder.HasIndex(p => p.Status);
    }
}
