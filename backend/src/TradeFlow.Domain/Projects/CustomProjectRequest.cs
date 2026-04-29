using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Events;

namespace TradeFlow.Domain.Projects;

public enum ProjectStatus
{
    Submitted,
    Quoted,
    DepositPaid,
    InProgress,
    Delivered,
    Cancelled
}

public class CustomProjectRequest : AggregateRoot<CustomProjectRequestId>
{
    public CustomerId CustomerId { get; private set; } = default!;
    public string Market { get; private set; } = default!;
    public string Timeframe { get; private set; } = default!;
    public string EntryConditions { get; private set; } = default!;
    public string ExitConditions { get; private set; } = default!;
    public string RiskManagement { get; private set; } = default!;
    public string? AdditionalNotes { get; private set; }
    public ProjectStatus Status { get; private set; }

    public decimal? QuotedPrice { get; private set; }
    public string? QuotedCurrency { get; private set; }
    public decimal? DepositAmount { get; private set; }
    public string? AdminNotes { get; private set; }
    public string? DeliveryFileUrl { get; private set; }
    public string? DeliveryNotes { get; private set; }

    public DateTime? QuotedAt { get; private set; }
    public DateTime? DepositPaidAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    private CustomProjectRequest() { }

    public static CustomProjectRequest Submit(
        CustomerId customerId,
        string market,
        string timeframe,
        string entryConditions,
        string exitConditions,
        string riskManagement,
        string? additionalNotes = null)
        => new()
        {
            Id = CustomProjectRequestId.New(),
            CustomerId = customerId,
            Market = market,
            Timeframe = timeframe,
            EntryConditions = entryConditions,
            ExitConditions = exitConditions,
            RiskManagement = riskManagement,
            AdditionalNotes = additionalNotes,
            Status = ProjectStatus.Submitted
        };

    public Result SendQuote(decimal price, string currency, string? adminNotes = null)
    {
        if (Status != ProjectStatus.Submitted)
            return Result.Failure("Request must be Submitted to send a quote.");

        QuotedPrice = price;
        QuotedCurrency = currency;
        DepositAmount = Math.Round(price * 0.5m, 2);
        AdminNotes = adminNotes;
        Status = ProjectStatus.Quoted;
        QuotedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProjectQuotedEvent(Id, CustomerId));
        return Result.Success();
    }

    public Result MarkDepositPaid()
    {
        if (Status != ProjectStatus.Quoted)
            return Result.Failure("Request must be Quoted before deposit payment.");

        Status = ProjectStatus.DepositPaid;
        DepositPaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result StartWork()
    {
        if (Status != ProjectStatus.DepositPaid)
            return Result.Failure("Deposit must be paid before starting work.");

        Status = ProjectStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Deliver(string fileUrl, string? deliveryNotes = null)
    {
        if (Status != ProjectStatus.InProgress)
            return Result.Failure("Project must be InProgress to deliver.");

        DeliveryFileUrl = fileUrl;
        DeliveryNotes = deliveryNotes;
        Status = ProjectStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProjectDeliveredEvent(Id, CustomerId));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is ProjectStatus.Delivered or ProjectStatus.Cancelled)
            return Result.Failure("Cannot cancel a delivered or already cancelled project.");

        Status = ProjectStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
