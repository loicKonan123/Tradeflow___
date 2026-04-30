using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Projects.Queries;

public record ProjectDto(
    Guid Id, string StrategyTitle, string Market, string Timeframe, string Status,
    string Indicators, string? StrategyType, string? BudgetRange,
    string? TradingViewChartUrl, DateTime? DesiredDeadline,
    string? AttachmentUrl, string? AttachmentName,
    decimal? QuotedPrice, string? QuotedCurrency, decimal? DepositAmount,
    string? AdminNotes, string? DeliveryNotes, DateTime CreatedAt, DateTime? DeliveredAt);

public record GetMyProjectsQuery(Guid CustomerId) : IRequest<IReadOnlyList<ProjectDto>>;

public class GetMyProjectsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetMyProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(GetMyProjectsQuery request, CancellationToken ct)
        => await db.CustomProjectRequests
            .Where(p => p.CustomerId == CustomerId.From(request.CustomerId))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(
                p.Id.Value, p.StrategyTitle, p.Market, p.Timeframe, p.Status.ToString(),
                p.Indicators, p.StrategyType, p.BudgetRange,
                p.TradingViewChartUrl, p.DesiredDeadline,
                p.AttachmentUrl, p.AttachmentName,
                p.QuotedPrice, p.QuotedCurrency, p.DepositAmount,
                p.AdminNotes, p.DeliveryNotes, p.CreatedAt, p.DeliveredAt))
            .ToListAsync(ct);
}
