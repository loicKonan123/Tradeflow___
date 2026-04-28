using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Licensing;

namespace TradeFlow.Application.Orders.Commands;

public record MarkOrderPaidCommand(string StripePaymentIntentId) : IRequest<Result>;

public class MarkOrderPaidCommandHandler(IApplicationDbContext context)
    : IRequestHandler<MarkOrderPaidCommand, Result>
{
    public async Task<Result> Handle(MarkOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.StripePaymentIntentId == request.StripePaymentIntentId, cancellationToken);

        if (order is null)
            return Result.Failure("Order not found for this payment intent.");

        var result = order.MarkAsPaid(request.StripePaymentIntentId);
        if (result.IsFailure)
            return result;

        // Create a license per order item
        foreach (var item in order.Items)
        {
            var license = License.Create(
                order.CustomerId,
                item.ProductId,
                order.Id,
                order.TradingViewUsername,
                LicenseType.Lifetime
            );
            context.Licenses.Add(license);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
