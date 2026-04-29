using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Projects.Commands;
using TradeFlow.Application.Projects.Queries;

namespace TradeFlow.Api.Controllers.Customer;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyProjects(CancellationToken ct)
    {
        if (!TryGetCustomerId(out var customerId)) return Unauthorized();
        var projects = await mediator.Send(new GetMyProjectsQuery(customerId), ct);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitProjectRequest request, CancellationToken ct)
    {
        if (!TryGetCustomerId(out var customerId)) return Unauthorized();
        var result = await mediator.Send(new SubmitProjectCommand(
            customerId, request.Market, request.Timeframe,
            request.EntryConditions, request.ExitConditions,
            request.RiskManagement, request.AdditionalNotes), ct);
        return result.IsSuccess ? Ok(new { projectId = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelProjectCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    private bool TryGetCustomerId(out Guid customerId)
    {
        customerId = Guid.Empty;
        var claim = User.FindFirst("customer_id")?.Value;
        return claim is not null && Guid.TryParse(claim, out customerId);
    }
}

public record SubmitProjectRequest(
    string Market, string Timeframe,
    string EntryConditions, string ExitConditions,
    string RiskManagement, string? AdditionalNotes);
