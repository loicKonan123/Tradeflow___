using MediatR;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Domain.Events;

public record OrderPaidEvent(OrderId OrderId, string StripePaymentIntentId) : INotification;
