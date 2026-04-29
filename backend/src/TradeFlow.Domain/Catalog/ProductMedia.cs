using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Catalog;

public enum MediaType { Image, Video }

public class ProductMedia : BaseEntity<ProductMediaId>
{
    public ProductId ProductId { get; private set; } = default!;
    public string Url { get; private set; } = default!;
    public MediaType Type { get; private set; }
    public int SortOrder { get; private set; }

    private ProductMedia() { }

    public static ProductMedia Create(ProductId productId, string url, MediaType type, int sortOrder = 0)
        => new()
        {
            Id = ProductMediaId.New(),
            ProductId = productId,
            Url = url,
            Type = type,
            SortOrder = sortOrder
        };
}
