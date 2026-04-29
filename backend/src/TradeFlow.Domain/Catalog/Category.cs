using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Catalog;

public class Category : BaseEntity<CategoryId>
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;

    private Category() { }

    public static Category Create(string name)
    {
        var slug = name.ToLowerInvariant().Replace(" ", "-");
        return new Category { Id = CategoryId.New(), Name = name, Slug = slug };
    }

    public void Update(string name)
    {
        Name = name;
        Slug = name.ToLowerInvariant().Replace(" ", "-");
        UpdatedAt = DateTime.UtcNow;
    }
}
