using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Application.Catalog.Queries;

public record ProductSummaryDto(
    Guid Id, string Title, string Slug, string ShortDescription,
    string Type, decimal Price, string Currency, string Status,
    string? CategoryName, int SalesCount, DateTime? PublishedAt);

public record GetProductsQuery(string? CategorySlug = null, string? Type = null) : IRequest<IReadOnlyList<ProductSummaryDto>>;

public class GetProductsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductSummaryDto>>
{
    public async Task<IReadOnlyList<ProductSummaryDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = db.Products
            .Include(p => p.Medias)
            .Where(p => p.Status == ProductStatus.Published)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.CategorySlug))
        {
            var cat = await db.Categories.FirstOrDefaultAsync(c => c.Slug == request.CategorySlug, ct);
            if (cat is not null)
                query = query.Where(p => p.CategoryId == cat.Id);
        }

        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<ProductType>(request.Type, out var type))
            query = query.Where(p => p.Type == type);

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new ProductSummaryDto(
                p.Id.Value, p.Title, p.Slug.Value, p.ShortDescription,
                p.Type.ToString(), p.Price, p.Currency, p.Status.ToString(),
                null, p.SalesCount, p.PublishedAt))
            .ToListAsync(ct);
    }
}
