using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Orders.Queries;

public record OrderItemDto(Guid ProductId, string ProductTitle, decimal UnitPrice, string Currency);
public record OrderDto(Guid Id, string OrderNumber, decimal Total, string Currency, string Status, DateTime CreatedAt, IReadOnlyList<OrderItemDto> Items);

public record GetMyOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;

public class GetMyOrdersQueryHandler(IApplicationDbContext db) : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken ct)
        => await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == CustomerId.From(request.CustomerId))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id.Value, o.OrderNumber, o.Total, o.Currency, o.Status.ToString(), o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(i.ProductId.Value, i.ProductTitle, i.UnitPrice, i.Currency)).ToList()))
            .ToListAsync(ct);
}
