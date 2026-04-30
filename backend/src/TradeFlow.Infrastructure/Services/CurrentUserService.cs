using Microsoft.AspNetCore.Http;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly System.Security.Claims.ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public string? FirebaseUid => _user?.FindFirst("user_id")?.Value;
    public string? Email => _user?.FindFirst("email")?.Value ?? _user?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    // TODO: replace with Firebase custom claim check before prod
    private static readonly HashSet<string> _adminEmails = ["devalinloic@gmail.com"];
    public bool IsAdmin =>
        (_user?.HasClaim(c => c.Type == "role" && c.Value == "admin") ?? false)
        || _adminEmails.Contains(Email ?? "");
    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;
}
