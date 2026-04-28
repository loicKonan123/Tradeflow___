using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Orders.Commands;
using TradeFlow.Application.Orders.Queries;

namespace TradeFlow.Api.Controllers.Customer;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        if (!TryGetCustomerId(out var customerId))
            return Unauthorized();

        var orders = await mediator.Send(new GetMyOrdersQuery(customerId), ct);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    private bool TryGetCustomerId(out Guid customerId)
    {
        // CustomerId resolved after sync — stored as claim after CustomerSyncMiddleware (Sprint auth)
        // For now parsed from header or default
        customerId = Guid.Empty;
        var claim = User.FindFirst("customer_id")?.Value;
        return claim is not null && Guid.TryParse(claim, out customerId);
    }
}
