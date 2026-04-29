using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Domain.Licensing;

public enum LicenseStatus { Active, Expired, Revoked }

public class License : AggregateRoot<LicenseId>
{
    public CustomerId CustomerId { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public OrderId OrderId { get; private set; } = default!;
    public string DownloadToken { get; private set; } = default!;
    public int DownloadCount { get; private set; }
    public int MaxDownloads { get; private set; } = 5;
    public LicenseStatus Status { get; private set; }

    private License() { }

    public static License Create(CustomerId customerId, ProductId productId, OrderId orderId)
        => new()
        {
            Id = LicenseId.New(),
            CustomerId = customerId,
            ProductId = productId,
            OrderId = orderId,
            DownloadToken = Guid.NewGuid().ToString("N"),
            Status = LicenseStatus.Active
        };

    public Result RecordDownload()
    {
        if (Status != LicenseStatus.Active)
            return Result.Failure("License is not active.");

        if (DownloadCount >= MaxDownloads)
            return Result.Failure("Download limit reached. Contact support to get a new link.");

        DownloadCount++;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Revoke()
    {
        if (Status == LicenseStatus.Revoked)
            return Result.Failure("License is already revoked.");

        Status = LicenseStatus.Revoked;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void ResetDownloadToken()
    {
        DownloadToken = Guid.NewGuid().ToString("N");
        DownloadCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }
}
