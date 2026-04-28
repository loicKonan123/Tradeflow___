using MediatR;
using TradeFlow.Domain.Licensing;

namespace TradeFlow.Domain.Events;

public record LicenseAccessGrantedEvent(LicenseId LicenseId) : INotification;
