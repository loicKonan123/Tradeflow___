using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? FirebaseUid => User?.FindFirst("user_id")?.Value;
    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
    public bool IsAdmin => User?.HasClaim(c => c.Type == "role" && c.Value == "admin") ?? false;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
