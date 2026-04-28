namespace TradeFlow.Domain.Pricing;

public sealed record Money(decimal Amount, Currency Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add amounts with different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money ApplyTaxRate(decimal rate) => new(Math.Round(Amount * (1 + rate), 2), Currency);

    public Money ApplyDiscount(decimal percentage) => new(Math.Round(Amount * (1 - percentage / 100), 2), Currency);

    public static Money Zero(Currency currency) => new(0m, currency);
}
