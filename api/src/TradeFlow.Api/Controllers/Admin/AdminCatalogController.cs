using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Catalog.Commands;
using TradeFlow.Application.Catalog.Queries;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog")]
[Authorize(Policy = "AdminOnly")]
public class AdminCatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminProductsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProductCommand(
            id, request.Title, request.ShortDescription, request.LongDescriptionMarkdown, request.BasePrice, request.Currency), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishProductCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ArchiveProductCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateProductRequest(string Title, string ShortDescription, string LongDescriptionMarkdown, decimal BasePrice, string Currency);
