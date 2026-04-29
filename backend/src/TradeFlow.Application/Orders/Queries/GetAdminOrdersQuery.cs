using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Common.Models;

namespace TradeFlow.Application.Orders.Queries;

public record AdminOrderDto(Guid Id, string OrderNumber, Guid CustomerId, string CustomerEmail, decimal Total, string Currency, string Status, DateTime CreatedAt);

public record GetAdminOrdersQuery(int Page = 1, int PageSize = 20, string? Status = null) : IRequest<PagedResult<AdminOrderDto>>;

public class GetAdminOrdersQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAdminOrdersQuery, PagedResult<AdminOrderDto>>
{
    public async Task<PagedResult<AdminOrderDto>> Handle(GetAdminOrdersQuery request, CancellationToken ct)
    {
        var query = db.Orders
            .Join(db.Customers, o => o.CustomerId, c => c.Id, (o, c) => new { o, c })
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<Domain.Orders.OrderStatus>(request.Status, out var status))
            query = query.Where(x => x.o.Status == status);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AdminOrderDto(
                x.o.Id.Value, x.o.OrderNumber, x.c.Id.Value, x.c.Email,
                x.o.Total, x.o.Currency, x.o.Status.ToString(), x.o.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<AdminOrderDto>(items, total, request.Page, request.PageSize);
    }
}
