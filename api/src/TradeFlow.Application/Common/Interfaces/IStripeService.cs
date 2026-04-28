namespace TradeFlow.Application.Common.Interfaces;

public interface IStripeService
{
    Task<string> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default);
    Task RefundAsync(string paymentIntentId, decimal amount, CancellationToken cancellationToken = default);
}
