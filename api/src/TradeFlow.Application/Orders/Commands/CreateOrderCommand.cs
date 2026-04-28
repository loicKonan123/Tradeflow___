using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Licensing;
using TradeFlow.Domain.Orders;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Application.Orders.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    string TradingViewUsername,
    List<OrderLineRequest> Lines,
    string Currency,
    string CountryCode
) : IRequest<Result<CreateOrderResult>>;

public record OrderLineRequest(Guid ProductId, int Quantity = 1);

public record CreateOrderResult(Guid OrderId, string OrderNumber, decimal Total, string StripeClientSecret);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TradingViewUsername).NotEmpty().MinimumLength(3).MaximumLength(30)
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("TradingView username can only contain letters, numbers and underscores.");
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Order must have at least one item.");
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.CountryCode).NotEmpty().Length(2);
    }
}

public class CreateOrderCommandHandler(IApplicationDbContext context, IOrderNumberService orderNumberService)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResult>>
{
    public async Task<Result<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customerId = CustomerId.From(request.CustomerId);
        var currency = new Currency(request.Currency.ToUpperInvariant());

        // Load products
        var productIds = request.Lines.Select(l => l.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id.Value))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
            return Result.Failure<CreateOrderResult>("One or more products not found.");

        // Build order items
        var orderId = OrderId.New();
        var items = request.Lines.Select(line =>
        {
            var product = products.First(p => p.Id.Value == line.ProductId);
            return OrderItem.Create(orderId, product.Id, product.Title, product.BasePrice, line.Quantity);
        }).ToList();

        // Calculate taxes based on country
        var taxRate = CalculateTaxRate(request.CountryCode);
        var subtotal = items.Aggregate(Money.Zero(currency), (sum, item) => sum.Add(item.LineTotal));
        var taxAmount = new Money(Math.Round(subtotal.Amount * taxRate, 2), currency);
        var discountAmount = Money.Zero(currency);

        var orderNumber = await orderNumberService.GenerateAsync(cancellationToken);
        var order = Order.Create(customerId, orderNumber, request.TradingViewUsername, items, taxAmount, discountAmount);

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        // Stripe PaymentIntent is created by the StripeService (called from controller)
        return Result.Success(new CreateOrderResult(order.Id.Value, order.OrderNumber, order.Total.Amount, string.Empty));
    }

    private static decimal CalculateTaxRate(string countryCode) => countryCode.ToUpperInvariant() switch
    {
        "CA" => 0.05m,   // TPS only (autres provinces par défaut)
        "QC" => 0.14975m, // TPS + TVQ
        "ON" or "NB" or "NS" or "PE" or "NL" => 0.13m, // HST
        _ => 0m
    };
}
