using MediatR;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Domain.Events;

public record ProductPublishedEvent(ProductId ProductId) : INotification;
