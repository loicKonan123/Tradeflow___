using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Common.Models;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Queries;

public record AdminProjectDto(
    Guid Id, Guid CustomerId, string CustomerEmail,
    string StrategyTitle, string Market, string Timeframe,
    string EntryConditions, string ExitConditions, string RiskManagement,
    string Indicators, string? StrategyType, string? BudgetRange,
    string? TradingViewChartUrl, DateTime? DesiredDeadline,
    string? AttachmentUrl, string? AttachmentName,
    string? AdditionalNotes, string Status,
    decimal? QuotedPrice, string? QuotedCurrency, string? AdminNotes,
    DateTime CreatedAt, DateTime? DeliveredAt);

public record GetAdminProjectsQuery(int Page = 1, int PageSize = 20, string? Status = null) : IRequest<PagedResult<AdminProjectDto>>;

public class GetAdminProjectsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAdminProjectsQuery, PagedResult<AdminProjectDto>>
{
    public async Task<PagedResult<AdminProjectDto>> Handle(GetAdminProjectsQuery request, CancellationToken ct)
    {
        var query = db.CustomProjectRequests
            .Join(db.Customers, p => p.CustomerId, c => c.Id, (p, c) => new { p, c })
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ProjectStatus>(request.Status, out var status))
            query = query.Where(x => x.p.Status == status);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AdminProjectDto(
                x.p.Id.Value, x.c.Id.Value, x.c.Email,
                x.p.StrategyTitle, x.p.Market, x.p.Timeframe,
                x.p.EntryConditions, x.p.ExitConditions, x.p.RiskManagement,
                x.p.Indicators, x.p.StrategyType, x.p.BudgetRange,
                x.p.TradingViewChartUrl, x.p.DesiredDeadline,
                x.p.AttachmentUrl, x.p.AttachmentName,
                x.p.AdditionalNotes, x.p.Status.ToString(),
                x.p.QuotedPrice, x.p.QuotedCurrency, x.p.AdminNotes,
                x.p.CreatedAt, x.p.DeliveredAt))
            .ToListAsync(ct);

        return new PagedResult<AdminProjectDto>(items, total, request.Page, request.PageSize);
    }
}
