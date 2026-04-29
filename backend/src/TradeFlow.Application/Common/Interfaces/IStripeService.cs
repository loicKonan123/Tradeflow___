namespace TradeFlow.Application.Common.Interfaces;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(
        string orderId,
        string orderNumber,
        IEnumerable<(string Name, decimal Price, string Currency)> items,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default);
}
