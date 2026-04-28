using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Catalog.Queries;

namespace TradeFlow.Api.Controllers.Public;

[ApiController]
[Route("api/catalog")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? type, CancellationToken ct)
    {
        var products = await mediator.Send(new GetProductsQuery(), ct);
        return Ok(products);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProduct(string slug, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductBySlugQuery(slug), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }
}
