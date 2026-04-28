using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Orders.Commands;

namespace TradeFlow.Api.Controllers.Webhooks;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        var webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");

        string payload;
        using (var reader = new StreamReader(Request.Body))
            payload = await reader.ReadToEndAsync(ct);

        var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(stripeSignature))
            return BadRequest("Missing Stripe-Signature header.");

        // TODO Sprint 3: validate HMAC with Stripe SDK
        // var stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, webhookSecret);

        // For now, parse minimal JSON to get event type and payment intent id
        var json = System.Text.Json.JsonDocument.Parse(payload);
        var eventType = json.RootElement.GetProperty("type").GetString();
        var paymentIntentId = json.RootElement
            .GetProperty("data").GetProperty("object").GetProperty("id").GetString();

        if (eventType == "payment_intent.succeeded" && !string.IsNullOrEmpty(paymentIntentId))
        {
            var result = await mediator.Send(new MarkOrderPaidCommand(paymentIntentId), ct);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });
        }

        return Ok();
    }
}
