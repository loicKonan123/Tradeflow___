using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Licensing.Commands;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/licenses")]
[Authorize(Policy = "AdminOnly")]
public class AdminLicensesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new RevokeLicenseCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
