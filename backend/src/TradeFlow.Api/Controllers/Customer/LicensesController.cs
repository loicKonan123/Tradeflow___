using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Licensing.Commands;
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
        if (!TryGetCustomerId(out var customerId)) return Unauthorized();
        var licenses = await mediator.Send(new GetMyLicensesQuery(customerId), ct);
        return Ok(licenses);
    }

    [HttpPost("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        if (!TryGetCustomerId(out var customerId)) return Unauthorized();
        var result = await mediator.Send(new DownloadLicenseCommand(id, customerId), ct);
        return result.IsSuccess ? Ok(new { url = result.Value }) : BadRequest(new { error = result.Error });
    }

    private bool TryGetCustomerId(out Guid customerId)
    {
        customerId = Guid.Empty;
        var claim = User.FindFirst("customer_id")?.Value;
        return claim is not null && Guid.TryParse(claim, out customerId);
    }
}
