using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Common.Models;

namespace TradeFlow.Application.Catalog.Queries;

public record GetAdminProductsQuery(int Page = 1, int PageSize = 20, string? Status = null) : IRequest<PagedResult<ProductSummaryDto>>;

public class GetAdminProductsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAdminProductsQuery, PagedResult<ProductSummaryDto>>
{
    public async Task<PagedResult<ProductSummaryDto>> Handle(GetAdminProductsQuery request, CancellationToken ct)
    {
        var query = db.Products.AsQueryable();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<Domain.Catalog.ProductStatus>(request.Status, out var status))
            query = query.Where(p => p.Status == status);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductSummaryDto(
                p.Id.Value, p.Title, p.Slug.Value, p.ShortDescription,
                p.Type.ToString(), p.Price, p.Currency, p.Status.ToString(),
                null, p.SalesCount, p.PublishedAt))
            .ToListAsync(ct);

        return new PagedResult<ProductSummaryDto>(items, total, request.Page, request.PageSize);
    }
}
