using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Customers;

public enum CustomerRole { Customer, Admin }

public class Customer : AggregateRoot<CustomerId>
{
    public string FirebaseUid { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? DisplayName { get; private set; }
    public string? CountryCode { get; private set; }
    public string PreferredCurrency { get; private set; } = "EUR";
    public string? TradingViewUsername { get; private set; }
    public CustomerRole Role { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Customer() { }

    public static Customer Create(string firebaseUid, string email, string? displayName)
    {
        return new Customer
        {
            Id = CustomerId.New(),
            FirebaseUid = firebaseUid,
            Email = email,
            DisplayName = displayName,
            Role = CustomerRole.Customer,
        };
    }

    public void UpdateProfile(string? displayName, string? countryCode, string? tradingViewUsername)
    {
        DisplayName = displayName;
        CountryCode = countryCode;
        TradingViewUsername = tradingViewUsername;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRole(CustomerRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }
}
