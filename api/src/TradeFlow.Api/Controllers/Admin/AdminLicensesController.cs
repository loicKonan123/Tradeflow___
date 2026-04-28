using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Licensing.Commands;
using TradeFlow.Application.Licensing.Queries;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/licenses")]
[Authorize(Policy = "AdminOnly")]
public class AdminLicensesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminLicensesQuery(status, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/grant")]
    public async Task<IActionResult> Grant(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GrantLicenseAccessCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, [FromBody] RevokeRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RevokeLicenseCommand(id, request.Reason), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record RevokeRequest(string Reason);
