using MediatR;

namespace TradeFlow.Domain.Common;

public abstract class AggregateRoot<TId> : BaseEntity<TId>, IAggregateRoot
{
    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
