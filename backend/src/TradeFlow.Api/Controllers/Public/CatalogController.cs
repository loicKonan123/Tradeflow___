using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Catalog.Queries;

namespace TradeFlow.Api.Controllers.Public;

[ApiController]
[Route("api/catalog")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] string? type, CancellationToken ct)
    {
        var products = await mediator.Send(new GetProductsQuery(category, type), ct);
        return Ok(products);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProduct(string slug, CancellationToken ct)
    {
        var product = await mediator.Send(new GetProductBySlugQuery(slug), ct);
        return product is null ? NotFound() : Ok(product);
    }
}
