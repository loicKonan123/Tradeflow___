using MediatR;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Domain.Events;

public record OrderPaidEvent(OrderId OrderId, CustomerId CustomerId) : INotification;
