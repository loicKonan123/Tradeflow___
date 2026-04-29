using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Projects.Commands;
using TradeFlow.Application.Projects.Queries;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/projects")]
[Authorize(Policy = "AdminOnly")]
public class AdminProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminProjectsQuery(page, pageSize, status), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/quote")]
    public async Task<IActionResult> SendQuote(Guid id, [FromBody] SendQuoteRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new SendQuoteCommand(id, request.Price, request.Currency, request.AdminNotes), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartWork(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new StartProjectWorkCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id, IFormFile file, [FromForm] string? deliveryNotes, CancellationToken ct)
    {
        if (file.Length == 0) return BadRequest(new { error = "File is empty." });
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new DeliverProjectCommand(id, stream, file.FileName, file.ContentType, deliveryNotes), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelProjectCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record SendQuoteRequest(decimal Price, string Currency, string? AdminNotes);
