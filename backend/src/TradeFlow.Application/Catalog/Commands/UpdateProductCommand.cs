using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Title,
    string ShortDescription,
    string LongDescriptionMarkdown,
    decimal Price,
    string Currency,
    Guid? CategoryId) : IRequest<Result>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

public class UpdateProductCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(request.Id), ct);
        if (product is null) return Result.Failure("Product not found.");

        var categoryId = request.CategoryId.HasValue ? CategoryId.From(request.CategoryId.Value) : null;
        product.UpdateDetails(request.Title, request.ShortDescription, request.LongDescriptionMarkdown, categoryId);

        var priceResult = product.UpdatePrice(request.Price, request.Currency);
        if (priceResult.IsFailure) return priceResult;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
