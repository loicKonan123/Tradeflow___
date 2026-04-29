using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;

namespace TradeFlow.Application.Catalog.Queries;

public record BacktestReportDto(
    Guid Id, string Title, decimal WinRate, decimal MaxDrawdown,
    decimal ProfitFactor, DateTime PeriodStart, DateTime PeriodEnd, string Markets, string? Notes);

public record ProductMediaDto(Guid Id, string Url, string Type, int SortOrder);

public record ProductDetailDto(
    Guid Id, string Title, string Slug, string ShortDescription,
    string LongDescriptionMarkdown, string Type, decimal Price, string Currency,
    string Status, string? CategoryName, int SalesCount, DateTime? PublishedAt,
    IReadOnlyList<ProductMediaDto> Medias,
    IReadOnlyList<BacktestReportDto> BacktestReports);

public record GetProductBySlugQuery(string Slug) : IRequest<ProductDetailDto?>;

public class GetProductBySlugQueryHandler(IApplicationDbContext db) : IRequestHandler<GetProductBySlugQuery, ProductDetailDto?>
{
    public async Task<ProductDetailDto?> Handle(GetProductBySlugQuery request, CancellationToken ct)
    {
        var product = await db.Products
            .Include(p => p.Medias)
            .Include(p => p.BacktestReports)
            .Where(p => p.Slug == Slug.From(request.Slug) && p.Status == ProductStatus.Published)
            .FirstOrDefaultAsync(ct);

        if (product is null) return null;

        return new ProductDetailDto(
            product.Id.Value, product.Title, product.Slug.Value,
            product.ShortDescription, product.LongDescriptionMarkdown,
            product.Type.ToString(), product.Price, product.Currency,
            product.Status.ToString(), null, product.SalesCount, product.PublishedAt,
            product.Medias.Select(m => new ProductMediaDto(m.Id.Value, m.Url, m.Type.ToString(), m.SortOrder)).ToList(),
            product.BacktestReports.Select(b => new BacktestReportDto(
                b.Id.Value, b.Title, b.WinRate, b.MaxDrawdown, b.ProfitFactor,
                b.PeriodStart, b.PeriodEnd, b.Markets, b.Notes)).ToList());
    }
}
