using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using TradeFlow.Application.Orders.Commands;

namespace TradeFlow.Api.Controllers.Webhooks;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController(IMediator mediator, IConfiguration config) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(ct);
        var webhookSecret = config["Stripe:WebhookSecret"]!;

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webhookSecret);
        }
        catch (StripeException)
        {
            return BadRequest();
        }

        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            if (session is null) return Ok();

            var paymentIntentId = session.PaymentIntentId ?? session.Id;
            await mediator.Send(new MarkOrderPaidCommand(paymentIntentId, session.Id), ct);
        }

        return Ok();
    }
}
