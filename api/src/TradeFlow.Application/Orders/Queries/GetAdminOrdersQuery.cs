using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Application.Orders.Queries;

public record GetAdminOrdersQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<OrderAdminDto>>;

public record OrderAdminDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string Status,
    string PaymentStatus,
    decimal Total,
    string Currency,
    string TradingViewUsername,
    string? StripePaymentIntentId,
    DateTime CreatedAt,
    DateTime? PaidAt,
    DateTime? FulfilledAt
);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class GetAdminOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAdminOrdersQuery, PagedResult<OrderAdminDto>>
{
    public async Task<PagedResult<OrderAdminDto>> Handle(GetAdminOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var status))
            query = query.Where(o => o.Status == status);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrderAdminDto(
                o.Id.Value,
                o.OrderNumber,
                o.CustomerId.Value,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.Total.Amount,
                o.Total.Currency.Code,
                o.TradingViewUsername,
                o.StripePaymentIntentId,
                o.CreatedAt,
                o.PaidAt,
                o.FulfilledAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderAdminDto>(items, total, request.Page, request.PageSize);
    }
}
