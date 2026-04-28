using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Application.Catalog.Queries;

public record GetProductsQuery(string? Category = null, ProductType? Type = null) : IRequest<List<ProductDto>>;

public record ProductDto(
    Guid Id,
    string Slug,
    string Title,
    string ShortDescription,
    string Type,
    decimal BasePrice,
    string Currency,
    string Status,
    int SalesCount,
    DateTime? PublishedAt
);

public class GetProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products
            .Where(p => p.Status == ProductStatus.Published);

        if (request.Type.HasValue)
            query = query.Where(p => p.Type == request.Type.Value);

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new ProductDto(
                p.Id.Value,
                p.Slug.Value,
                p.Title,
                p.ShortDescription,
                p.Type.ToString(),
                p.BasePrice.Amount,
                p.BasePrice.Currency.Code,
                p.Status.ToString(),
                p.SalesCount,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
