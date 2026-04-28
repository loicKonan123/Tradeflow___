using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Domain.Orders;

public class OrderItem : BaseEntity<Guid>
{
    public OrderId OrderId { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public string ProductTitle { get; private set; } = default!;
    public Money UnitPrice { get; private set; } = default!;
    public int Quantity { get; private set; }
    public Money LineTotal { get; private set; } = default!;

    private OrderItem() { }

    public static OrderItem Create(OrderId orderId, ProductId productId, string productTitle, Money unitPrice, int quantity = 1)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            ProductTitle = productTitle,
            UnitPrice = unitPrice,
            Quantity = quantity,
            LineTotal = new Money(unitPrice.Amount * quantity, unitPrice.Currency),
        };
    }
}
