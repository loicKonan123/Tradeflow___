using FluentValidation;
using MediatR;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record CreateProductCommand(
    string Title,
    string ShortDescription,
    string LongDescriptionMarkdown,
    string Type,
    decimal Price,
    string Currency,
    Guid? CategoryId) : IRequest<Result<Guid>>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Type).Must(t => Enum.TryParse<ProductType>(t, out _))
            .WithMessage("Type must be Indicator, Strategy, or Bundle.");
    }
}

public class CreateProductCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var type = Enum.Parse<ProductType>(request.Type);
        var categoryId = request.CategoryId.HasValue ? CategoryId.From(request.CategoryId.Value) : null;

        var result = Product.Create(
            request.Title,
            request.ShortDescription,
            type,
            request.Price,
            request.Currency,
            categoryId);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error!);

        var product = result.Value;
        product.UpdateDetails(request.Title, request.ShortDescription, request.LongDescriptionMarkdown, categoryId);

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return Result.Success(product.Id.Value);
    }
}
