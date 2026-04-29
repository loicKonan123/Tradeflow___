using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, v => ProductId.From(v))
            .HasColumnName("id");

        builder.OwnsOne(p => p.Slug, slug =>
        {
            slug.Property(s => s.Value).IsRequired().HasMaxLength(220).HasColumnName("slug");
            slug.HasIndex(s => s.Value).IsUnique();
        });

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
        builder.Property(p => p.ShortDescription).IsRequired().HasMaxLength(500).HasColumnName("short_description");
        builder.Property(p => p.LongDescriptionMarkdown).HasColumnName("long_description_markdown");
        builder.Property(p => p.Type).HasConversion<string>().HasColumnName("type");
        builder.Property(p => p.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").HasColumnName("price");
        builder.Property(p => p.Currency).HasMaxLength(3).HasColumnName("currency");
        builder.Property(p => p.FileUrl).HasMaxLength(500).HasColumnName("file_url");
        builder.Property(p => p.SalesCount).HasColumnName("sales_count");
        builder.Property(p => p.PublishedAt).HasColumnName("published_at");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.Property(p => p.CategoryId)
            .HasConversion(id => id == null ? (Guid?)null : id.Value, v => v.HasValue ? CategoryId.From(v.Value) : null)
            .HasColumnName("category_id");

        builder.HasMany(p => p.Medias).WithOne().HasForeignKey(m => m.ProductId);
        builder.HasMany(p => p.BacktestReports).WithOne().HasForeignKey(b => b.ProductId);

        builder.Ignore(p => p.DomainEvents);
    }
}
