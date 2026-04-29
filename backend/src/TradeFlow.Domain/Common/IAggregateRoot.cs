using MediatR;

namespace TradeFlow.Domain.Common;

public interface IAggregateRoot
{
    IReadOnlyList<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}
