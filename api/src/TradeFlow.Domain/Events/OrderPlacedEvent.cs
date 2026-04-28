using MediatR;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Domain.Events;

public record OrderPlacedEvent(OrderId OrderId) : INotification;
