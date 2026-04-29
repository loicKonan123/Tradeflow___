using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Orders;

public sealed record OrderItemId(Guid Value)
{
    public static OrderItemId New() => new(Guid.NewGuid());
    public static OrderItemId From(Guid value) => new(value);
}

public class OrderItem : BaseEntity<OrderItemId>
{
    public OrderId OrderId { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public string ProductTitle { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = default!;

    private OrderItem() { }

    public static OrderItem Create(OrderId orderId, ProductId productId, string productTitle, decimal unitPrice, string currency)
        => new()
        {
            Id = OrderItemId.New(),
            OrderId = orderId,
            ProductId = productId,
            ProductTitle = productTitle,
            UnitPrice = unitPrice,
            Currency = currency
        };
}
