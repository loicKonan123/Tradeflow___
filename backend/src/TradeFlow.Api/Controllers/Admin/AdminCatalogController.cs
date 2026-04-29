using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Catalog.Commands;
using TradeFlow.Application.Catalog.Queries;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize(Policy = "AdminOnly")]
public class AdminCatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAdminProductsQuery(page, pageSize, status), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateProductCommand(
            request.Title, request.ShortDescription, request.LongDescriptionMarkdown,
            request.Type, request.Price, request.Currency, request.CategoryId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProductCommand(
            id, request.Title, request.ShortDescription, request.LongDescriptionMarkdown,
            request.Price, request.Currency, request.CategoryId), ct);
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

    [HttpPost("{id:guid}/file")]
    public async Task<IActionResult> UploadFile(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) return BadRequest(new { error = "File is empty." });
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new SetProductFileCommand(id, stream, file.FileName, file.ContentType), ct);
        return result.IsSuccess ? Ok(new { url = result.Value }) : BadRequest(new { error = result.Error });
    }
}

public record CreateProductRequest(string Title, string ShortDescription, string LongDescriptionMarkdown, string Type, decimal Price, string Currency, Guid? CategoryId);
public record UpdateProductRequest(string Title, string ShortDescription, string LongDescriptionMarkdown, decimal Price, string Currency, Guid? CategoryId);
