using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Events;

namespace TradeFlow.Domain.Orders;

public enum OrderStatus { Pending, Paid, Fulfilled, Refunded }

public class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _items = [];

    public CustomerId CustomerId { get; private set; } = default!;
    public string OrderNumber { get; private set; } = default!;
    public decimal Total { get; private set; }
    public string Currency { get; private set; } = "EUR";
    public OrderStatus Status { get; private set; }
    public string? StripePaymentIntentId { get; private set; }
    public string? StripeSessionId { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(CustomerId customerId, string orderNumber, string currency = "EUR")
        => new()
        {
            Id = OrderId.New(),
            CustomerId = customerId,
            OrderNumber = orderNumber,
            Currency = currency,
            Status = OrderStatus.Pending
        };

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
        Total = _items.Sum(i => i.UnitPrice);
        UpdatedAt = DateTime.UtcNow;
    }

    public Result MarkAsPaid(string paymentIntentId)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure("Order is not in Pending status.");

        Status = OrderStatus.Paid;
        StripePaymentIntentId = paymentIntentId;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderPaidEvent(Id, CustomerId));
        return Result.Success();
    }

    public Result MarkAsFulfilled()
    {
        if (Status != OrderStatus.Paid)
            return Result.Failure("Order must be Paid before fulfillment.");

        Status = OrderStatus.Fulfilled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void SetStripeSession(string sessionId)
    {
        StripeSessionId = sessionId;
        UpdatedAt = DateTime.UtcNow;
    }
}
