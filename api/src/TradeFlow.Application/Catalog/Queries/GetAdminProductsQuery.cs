using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Application.Orders.Queries;

namespace TradeFlow.Application.Catalog.Queries;

public record GetAdminProductsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<ProductAdminDto>>;

public record ProductAdminDto(
    Guid Id,
    string Slug,
    string Title,
    string Type,
    decimal BasePrice,
    string Currency,
    string Status,
    int SalesCount,
    DateTime CreatedAt,
    DateTime? PublishedAt
);

public class GetAdminProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAdminProductsQuery, PagedResult<ProductAdminDto>>
{
    public async Task<PagedResult<ProductAdminDto>> Handle(GetAdminProductsQuery request, CancellationToken cancellationToken)
    {
        var total = await context.Products.CountAsync(cancellationToken);

        var items = await context.Products
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductAdminDto(
                p.Id.Value,
                p.Slug.Value,
                p.Title,
                p.Type.ToString(),
                p.BasePrice.Amount,
                p.BasePrice.Currency.Code,
                p.Status.ToString(),
                p.SalesCount,
                p.CreatedAt,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductAdminDto>(items, total, request.Page, request.PageSize);
    }
}
