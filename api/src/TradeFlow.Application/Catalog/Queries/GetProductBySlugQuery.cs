using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Queries;

public record GetProductBySlugQuery(string Slug) : IRequest<Result<ProductDetailDto>>;

public record ProductDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string ShortDescription,
    string LongDescriptionMarkdown,
    string Type,
    decimal BasePrice,
    string Currency,
    string Status,
    int SalesCount,
    DateTime? PublishedAt
);

public class GetProductBySlugQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductBySlugQuery, Result<ProductDetailDto>>
{
    public async Task<Result<ProductDetailDto>> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Slug.Value == request.Slug && p.Status == ProductStatus.Published, cancellationToken);

        if (product is null)
            return Result.Failure<ProductDetailDto>($"Product '{request.Slug}' not found.");

        return Result.Success(new ProductDetailDto(
            product.Id.Value,
            product.Slug.Value,
            product.Title,
            product.ShortDescription,
            product.LongDescriptionMarkdown,
            product.Type.ToString(),
            product.BasePrice.Amount,
            product.BasePrice.Currency.Code,
            product.Status.ToString(),
            product.SalesCount,
            product.PublishedAt
        ));
    }
}
