using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Catalog.Queries;

namespace TradeFlow.Api.Controllers.Public;

[ApiController]
[Route("api/categories")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await mediator.Send(new GetCategoriesQuery(), ct);
        return Ok(categories);
    }
}
