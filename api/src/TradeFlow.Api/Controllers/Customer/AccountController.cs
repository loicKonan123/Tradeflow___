using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Customers.Commands;

namespace TradeFlow.Api.Controllers.Customer;

[ApiController]
[Route("api/account")]
[Authorize]
public class AccountController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("sync")]
    public async Task<IActionResult> Sync(CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.FirebaseUid is null)
            return Unauthorized();

        var result = await mediator.Send(
            new SyncCustomerCommand(currentUser.FirebaseUid, currentUser.Email!, null), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
