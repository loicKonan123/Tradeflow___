namespace TradeFlow.Domain.Pricing;

public sealed record Currency(string Code)
{
    public static readonly Currency EUR = new("EUR");
    public static readonly Currency USD = new("USD");
    public static readonly Currency CAD = new("CAD");
    public static readonly Currency XOF = new("XOF");
}
