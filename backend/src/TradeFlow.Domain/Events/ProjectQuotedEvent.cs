using MediatR;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Domain.Events;

public record ProjectQuotedEvent(CustomProjectRequestId ProjectId, CustomerId CustomerId) : INotification;
