using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog")]
[Authorize(Policy = "AdminOnly")]
public class AdminCatalogController : ControllerBase
{
    // Sprint 2 : CRUD produits admin
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { message = "Admin catalog ready" });
}
