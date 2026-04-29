namespace TradeFlow.Domain.Licensing;

public sealed record LicenseId(Guid Value)
{
    public static LicenseId New() => new(Guid.NewGuid());
    public static LicenseId From(Guid value) => new(value);
}
