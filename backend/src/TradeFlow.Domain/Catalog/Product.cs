using TradeFlow.Domain.Common;
using TradeFlow.Domain.Events;

namespace TradeFlow.Domain.Catalog;

public enum ProductType { Indicator, Strategy, Bundle }
public enum ProductStatus { Draft, Published, Archived }

public class Product : AggregateRoot<ProductId>
{
    private readonly List<ProductMedia> _medias = [];
    private readonly List<BacktestReport> _backtestReports = [];

    public Slug Slug { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string ShortDescription { get; private set; } = default!;
    public string LongDescriptionMarkdown { get; private set; } = string.Empty;
    public ProductType Type { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "EUR";
    public ProductStatus Status { get; private set; }
    public CategoryId? CategoryId { get; private set; }
    public string? FileUrl { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int SalesCount { get; private set; }

    public IReadOnlyList<ProductMedia> Medias => _medias.AsReadOnly();
    public IReadOnlyList<BacktestReport> BacktestReports => _backtestReports.AsReadOnly();

    private Product() { }

    public static Result<Product> Create(
        string title,
        string shortDescription,
        ProductType type,
        decimal price,
        string currency = "EUR",
        CategoryId? categoryId = null)
    {
        var slugResult = Slug.FromTitle(title);
        if (slugResult.IsFailure)
            return Result.Failure<Product>(slugResult.Error!);

        if (price < 0)
            return Result.Failure<Product>("Price cannot be negative.");

        var product = new Product
        {
            Id = ProductId.New(),
            Title = title,
            ShortDescription = shortDescription,
            Slug = slugResult.Value,
            Type = type,
            Price = price,
            Currency = currency,
            CategoryId = categoryId,
            Status = ProductStatus.Draft
        };

        return Result.Success(product);
    }

    public Result Publish()
    {
        if (Status == ProductStatus.Published)
            return Result.Failure("Product is already published.");

        if (string.IsNullOrWhiteSpace(FileUrl))
            return Result.Failure("Cannot publish a product without a file.");

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

    public void UpdateDetails(string title, string shortDescription, string longDescriptionMarkdown, CategoryId? categoryId)
    {
        Title = title;
        ShortDescription = shortDescription;
        LongDescriptionMarkdown = longDescriptionMarkdown;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result UpdatePrice(decimal price, string currency)
    {
        if (price < 0)
            return Result.Failure("Price cannot be negative.");

        Price = price;
        Currency = currency;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void SetFileUrl(string fileUrl)
    {
        FileUrl = fileUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMedia(ProductMedia media)
    {
        _medias.Add(media);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMedia(ProductMediaId mediaId)
    {
        var media = _medias.FirstOrDefault(m => m.Id == mediaId);
        if (media is not null)
        {
            _medias.Remove(media);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AddBacktestReport(BacktestReport report)
    {
        _backtestReports.Add(report);
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementSalesCount()
    {
        SalesCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}
