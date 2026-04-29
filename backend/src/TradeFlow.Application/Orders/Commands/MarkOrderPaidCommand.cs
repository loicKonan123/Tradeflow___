using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Licensing;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Application.Orders.Commands;

public record MarkOrderPaidCommand(string PaymentIntentId, string? SessionId = null) : IRequest<Result>;

public class MarkOrderPaidCommandHandler(IApplicationDbContext db) : IRequestHandler<MarkOrderPaidCommand, Result>
{
    public async Task<Result> Handle(MarkOrderPaidCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.StripeSessionId == request.SessionId
                || o.StripePaymentIntentId == request.PaymentIntentId, ct);

        if (order is null) return Result.Failure("Order not found.");
        if (order.Status != OrderStatus.Pending) return Result.Success();

        var paidResult = order.MarkAsPaid(request.PaymentIntentId);
        if (paidResult.IsFailure) return paidResult;

        foreach (var item in order.Items)
        {
            var license = License.Create(order.CustomerId, item.ProductId, order.Id);
            db.Licenses.Add(license);
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
