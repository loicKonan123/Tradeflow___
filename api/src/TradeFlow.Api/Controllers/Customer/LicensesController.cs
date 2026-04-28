using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Licensing.Queries;

namespace TradeFlow.Api.Controllers.Customer;

[ApiController]
[Route("api/licenses")]
[Authorize]
public class LicensesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyLicenses(CancellationToken ct)
    {
        var claim = User.FindFirst("customer_id")?.Value;
        if (claim is null || !Guid.TryParse(claim, out var customerId))
            return Unauthorized();

        var licenses = await mediator.Send(new GetMyLicensesQuery(customerId), ct);
        return Ok(licenses);
    }
}
