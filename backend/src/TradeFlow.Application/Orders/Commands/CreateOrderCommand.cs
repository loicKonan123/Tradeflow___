using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Application.Orders.Commands;

public record CreateOrderCommand(Guid CustomerId, IReadOnlyList<Guid> ProductIds, string Currency = "EUR") : IRequest<Result<CreateOrderResult>>;

public record CreateOrderResult(Guid OrderId, string OrderNumber, string CheckoutUrl);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ProductIds).NotEmpty().WithMessage("At least one product is required.");
    }
}

public class CreateOrderCommandHandler(
    IApplicationDbContext db,
    IOrderNumberService orderNumberService,
    IStripeService stripeService) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResult>>
{
    public async Task<Result<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var products = await db.Products
            .Where(p => request.ProductIds.Select(ProductId.From).Contains(p.Id)
                        && p.Status == ProductStatus.Published)
            .ToListAsync(ct);

        if (products.Count != request.ProductIds.Count)
            return Result.Failure<CreateOrderResult>("One or more products are unavailable.");

        var alreadyOwned = await db.Licenses
            .Where(l => l.CustomerId == CustomerId.From(request.CustomerId)
                        && request.ProductIds.Select(ProductId.From).Contains(l.ProductId)
                        && l.Status == Domain.Licensing.LicenseStatus.Active)
            .AnyAsync(ct);

        if (alreadyOwned)
            return Result.Failure<CreateOrderResult>("You already own one or more of these products.");

        var orderNumber = await orderNumberService.GenerateAsync(ct);
        var order = Order.Create(CustomerId.From(request.CustomerId), orderNumber, request.Currency);

        foreach (var product in products)
        {
            var item = OrderItem.Create(order.Id, product.Id, product.Title, product.Price, product.Currency);
            order.AddItem(item);
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        var lineItems = products.Select(p => (p.Title, p.Price, p.Currency));
        var checkoutUrl = await stripeService.CreateCheckoutSessionAsync(
            order.Id.Value.ToString(),
            order.OrderNumber,
            lineItems,
            $"http://localhost:3000/dashboard/orders?success=true",
            $"http://localhost:3000/dashboard/orders?cancelled=true",
            ct);

        return Result.Success(new CreateOrderResult(order.Id.Value, order.OrderNumber, checkoutUrl));
    }
}
