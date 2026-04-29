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

        var result = await mediator.Send(new SyncCustomerCommand(currentUser.FirebaseUid, currentUser.Email!, null), ct);
        return result.IsSuccess ? Ok(new { customerId = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        if (!TryGetCustomerId(out var customerId)) return Unauthorized();

        var result = await mediator.Send(new UpdateCustomerProfileCommand(customerId, request.DisplayName, request.TradingViewUsername), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    private bool TryGetCustomerId(out Guid customerId)
    {
        customerId = Guid.Empty;
        var claim = User.FindFirst("customer_id")?.Value;
        return claim is not null && Guid.TryParse(claim, out customerId);
    }
}

public record UpdateProfileRequest(string? DisplayName, string? TradingViewUsername);
