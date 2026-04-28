using FluentValidation;
using MediatR;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Pricing;

namespace TradeFlow.Application.Catalog.Commands;

public record CreateProductCommand(
    string Title,
    string ShortDescription,
    string LongDescriptionMarkdown,
    string Type,
    decimal BasePrice,
    string Currency
) : IRequest<Result<Guid>>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Type).NotEmpty()
            .Must(t => Enum.TryParse<ProductType>(t, true, out _))
            .WithMessage("Type must be Indicator, Strategy or Bundle.");
    }
}

public class CreateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ProductType>(request.Type, true, out var type))
            return Result.Failure<Guid>("Invalid product type.");

        var money = new Money(request.BasePrice, new Currency(request.Currency.ToUpperInvariant()));

        var result = Product.Create(request.Title, request.ShortDescription, type, money);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error!);

        var product = result.Value;

        if (!string.IsNullOrWhiteSpace(request.LongDescriptionMarkdown))
            product.UpdateDescription(request.LongDescriptionMarkdown);

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id.Value);
    }
}
