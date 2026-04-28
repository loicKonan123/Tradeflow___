using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Application.Catalog.Commands;

public record UpdateProductCommand(
    Guid ProductId,
    string Title,
    string ShortDescription,
    string LongDescriptionMarkdown,
    decimal BasePrice,
    string Currency
) : IRequest<Result>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

public class UpdateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id.Value == request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        product.UpdateDetails(request.Title, request.ShortDescription, request.LongDescriptionMarkdown);

        var priceResult = product.UpdatePrice(new Money(request.BasePrice, new Currency(request.Currency.ToUpperInvariant())));
        if (priceResult.IsFailure)
            return priceResult;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
