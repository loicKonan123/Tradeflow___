using TradeFlow.Domain.Common;
using TradeFlow.Domain.Pricing;
using TradeFlow.Domain.Events;

namespace TradeFlow.Domain.Catalog;

public enum ProductType { Indicator, Strategy, Bundle }
public enum ProductStatus { Draft, Published, Archived }

public class Product : AggregateRoot<ProductId>
{
    public Slug Slug { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string ShortDescription { get; private set; } = default!;
    public string LongDescriptionMarkdown { get; private set; } = string.Empty;
    public ProductType Type { get; private set; }
    public Money BasePrice { get; private set; } = default!;
    public ProductStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int SalesCount { get; private set; }

    private Product() { }

    public static Result<Product> Create(string title, string shortDescription, ProductType type, Money basePrice)
    {
        var slugResult = Slug.FromTitle(title);
        if (slugResult.IsFailure)
            return Result.Failure<Product>(slugResult.Error!);

        var product = new Product
        {
            Id = ProductId.New(),
            Title = title,
            ShortDescription = shortDescription,
            Slug = slugResult.Value,
            Type = type,
            BasePrice = basePrice,
            Status = ProductStatus.Draft,
        };

        return Result.Success(product);
    }

    public Result Publish()
    {
        if (Status == ProductStatus.Published)
            return Result.Failure("Product is already published.");

        Status = ProductStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProductPublishedEvent(Id));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure("Product is already archived.");

        Status = ProductStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            return Result.Failure("Price must be greater than zero.");

        BasePrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void UpdateDescription(string longDescriptionMarkdown)
    {
        LongDescriptionMarkdown = longDescriptionMarkdown;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string shortDescription, string longDescriptionMarkdown)
    {
        Title = title;
        ShortDescription = shortDescription;
        LongDescriptionMarkdown = longDescriptionMarkdown;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementSalesCount()
    {
        SalesCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}
