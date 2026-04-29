using MediatR;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Domain.Events;

public record ProjectDeliveredEvent(CustomProjectRequestId ProjectId, CustomerId CustomerId) : INotification;
