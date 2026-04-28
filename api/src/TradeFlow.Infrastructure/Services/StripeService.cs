using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

// Stub — sera implémenté avec Stripe SDK en Sprint 3
public class StripeService : IStripeService
{
    public Task<string> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // TODO Sprint 3: Stripe.PaymentIntentService.CreateAsync(...)
        return Task.FromResult($"pi_stub_{orderId:N}");
    }

    public Task RefundAsync(string paymentIntentId, decimal amount, CancellationToken cancellationToken = default)
    {
        // TODO Sprint 3: Stripe.RefundService.CreateAsync(...)
        return Task.CompletedTask;
    }
}
