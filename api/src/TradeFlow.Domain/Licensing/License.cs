using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Events;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Domain.Licensing;

public enum LicenseType { Lifetime, TimeLimited }
public enum LicenseStatus { Pending, Active, Revoked, Expired }

public class License : AggregateRoot<LicenseId>
{
    public CustomerId CustomerId { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public OrderId SourceOrderId { get; private set; } = default!;
    public string GrantedToTvUsername { get; private set; } = default!;
    public LicenseType Type { get; private set; }
    public LicenseStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? AccessGrantedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevocationReason { get; private set; }

    private License() { }

    public static License Create(CustomerId customerId, ProductId productId, OrderId orderId,
        string tvUsername, LicenseType type, DateTime? expiresAt = null)
    {
        return new License
        {
            Id = LicenseId.New(),
            CustomerId = customerId,
            ProductId = productId,
            SourceOrderId = orderId,
            GrantedToTvUsername = tvUsername,
            Type = type,
            Status = LicenseStatus.Pending,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        };
    }

    public Result MarkAccessGranted()
    {
        if (Status != LicenseStatus.Pending)
            return Result.Failure("License is not in pending state.");

        Status = LicenseStatus.Active;
        AccessGrantedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new LicenseAccessGrantedEvent(Id));
        return Result.Success();
    }

    public Result Revoke(string reason)
    {
        if (Status == LicenseStatus.Revoked)
            return Result.Failure("License is already revoked.");

        Status = LicenseStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
