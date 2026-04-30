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
    public string StrategyTitle { get; private set; } = default!;
    public string Market { get; private set; } = default!;
    public string Timeframe { get; private set; } = default!;
    public string EntryConditions { get; private set; } = default!;
    public string ExitConditions { get; private set; } = default!;
    public string RiskManagement { get; private set; } = default!;
    public string Indicators { get; private set; } = default!;
    public string? AdditionalNotes { get; private set; }
    public string? StrategyType { get; private set; }
    public string? TradingViewChartUrl { get; private set; }
    public string? BudgetRange { get; private set; }
    public DateTime? DesiredDeadline { get; private set; }
    public string? AttachmentUrl { get; private set; }
    public string? AttachmentName { get; private set; }
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
        string strategyTitle,
        string market,
        string timeframe,
        string entryConditions,
        string exitConditions,
        string riskManagement,
        string indicators,
        string? additionalNotes = null,
        string? strategyType = null,
        string? tradingViewChartUrl = null,
        string? budgetRange = null,
        DateTime? desiredDeadline = null)
        => new()
        {
            Id = CustomProjectRequestId.New(),
            CustomerId = customerId,
            StrategyTitle = strategyTitle,
            Market = market,
            Timeframe = timeframe,
            EntryConditions = entryConditions,
            ExitConditions = exitConditions,
            RiskManagement = riskManagement,
            Indicators = indicators,
            AdditionalNotes = additionalNotes,
            StrategyType = strategyType,
            TradingViewChartUrl = tradingViewChartUrl,
            BudgetRange = budgetRange,
            DesiredDeadline = desiredDeadline,
            Status = ProjectStatus.Submitted
        };

    public Result AddAttachment(string url, string name)
    {
        AttachmentUrl = url;
        AttachmentName = name;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

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
