namespace TradeFlow.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? FirebaseUid { get; }
    string? Email { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
}
