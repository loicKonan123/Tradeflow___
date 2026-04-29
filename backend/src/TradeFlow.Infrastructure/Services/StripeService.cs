using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

public class StripeService(IConfiguration config) : IStripeService
{
    public async Task<string> CreateCheckoutSessionAsync(
        string orderId,
        string orderNumber,
        IEnumerable<(string Name, decimal Price, string Currency)> items,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        var lineItems = items.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = item.Currency.ToLower(),
                UnitAmountDecimal = item.Price * 100,
                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Name }
            },
            Quantity = 1
        }).ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string> { { "order_id", orderId }, { "order_number", orderNumber } }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
        return session.Url;
    }
}
