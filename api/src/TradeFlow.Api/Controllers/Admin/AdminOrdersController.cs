using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Orders.Queries;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Policy = "AdminOnly")]
public class AdminOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminOrdersQuery(status, page, pageSize), ct);
        return Ok(result);
    }
}
