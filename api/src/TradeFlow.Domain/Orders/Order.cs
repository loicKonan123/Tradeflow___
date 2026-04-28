using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Events;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Domain.Orders;

public enum OrderStatus { Pending, Paid, Fulfilled, Refunded, Cancelled }
public enum PaymentStatus { Unpaid, Paid, Refunded, Failed }

public class Order : AggregateRoot<OrderId>
{
    public string OrderNumber { get; private set; } = default!;
    public CustomerId CustomerId { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Money Subtotal { get; private set; } = default!;
    public Money TaxAmount { get; private set; } = default!;
    public Money DiscountAmount { get; private set; } = default!;
    public Money Total { get; private set; } = default!;
    public string TradingViewUsername { get; private set; } = default!;
    public string? StripePaymentIntentId { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? FulfilledAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(CustomerId customerId, string orderNumber, string tradingViewUsername,
        List<OrderItem> items, Money taxAmount, Money discountAmount)
    {
        var subtotal = items.Aggregate(
            Money.Zero(items[0].UnitPrice.Currency),
            (sum, item) => sum.Add(item.LineTotal));

        var total = subtotal.Add(taxAmount);
        if (discountAmount.Amount > 0)
            total = new Money(total.Amount - discountAmount.Amount, total.Currency);

        var order = new Order
        {
            Id = OrderId.New(),
            OrderNumber = orderNumber,
            CustomerId = customerId,
            TradingViewUsername = tradingViewUsername,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DiscountAmount = discountAmount,
            Total = total,
        };

        order._items.AddRange(items);
        order.RaiseDomainEvent(new OrderPlacedEvent(order.Id));
        return order;
    }

    public Result MarkAsPaid(string stripePaymentIntentId)
    {
        if (PaymentStatus == PaymentStatus.Paid)
            return Result.Failure("Order is already paid.");

        StripePaymentIntentId = stripePaymentIntentId;
        PaymentStatus = PaymentStatus.Paid;
        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderPaidEvent(Id, stripePaymentIntentId));
        return Result.Success();
    }

    public Result MarkAsFulfilled()
    {
        if (Status != OrderStatus.Paid)
            return Result.Failure("Order must be paid before fulfillment.");

        Status = OrderStatus.Fulfilled;
        FulfilledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
