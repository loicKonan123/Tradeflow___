using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Orders.Queries;

public record GetMyOrdersQuery(Guid CustomerId) : IRequest<List<OrderSummaryDto>>;

public record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string Status,
    string PaymentStatus,
    decimal Total,
    string Currency,
    int ItemCount,
    DateTime CreatedAt,
    DateTime? PaidAt
);

public class GetMyOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyOrdersQuery, List<OrderSummaryDto>>
{
    public async Task<List<OrderSummaryDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var customerId = CustomerId.From(request.CustomerId);

        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto(
                o.Id.Value,
                o.OrderNumber,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.Total.Amount,
                o.Total.Currency.Code,
                o.Items.Count,
                o.CreatedAt,
                o.PaidAt
            ))
            .ToListAsync(cancellationToken);
    }
}
