using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Customers;

public class Customer : BaseEntity<CustomerId>
{
    public string FirebaseUid { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? DisplayName { get; private set; }
    public string? TradingViewUsername { get; private set; }
    public bool IsAdmin { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Customer() { }

    public static Customer Create(string firebaseUid, string email, string? displayName = null)
        => new()
        {
            Id = CustomerId.New(),
            FirebaseUid = firebaseUid,
            Email = email,
            DisplayName = displayName
        };

    public void UpdateProfile(string? displayName, string? tradingViewUsername)
    {
        DisplayName = displayName;
        TradingViewUsername = tradingViewUsername;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
