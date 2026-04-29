using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, v => CategoryId.From(v))
            .HasColumnName("id");
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(120).HasColumnName("slug");
        builder.HasIndex(c => c.Slug).IsUnique();
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
    }
}
