using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Licensing.Queries;

public record GetMyLicensesQuery(Guid CustomerId) : IRequest<List<LicenseDto>>;

public record LicenseDto(
    Guid Id,
    Guid ProductId,
    string ProductTitle,
    string TvUsername,
    string Type,
    string Status,
    DateTime IssuedAt,
    DateTime? ExpiresAt,
    DateTime? AccessGrantedAt
);

public class GetMyLicensesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyLicensesQuery, List<LicenseDto>>
{
    public async Task<List<LicenseDto>> Handle(GetMyLicensesQuery request, CancellationToken cancellationToken)
    {
        var customerId = CustomerId.From(request.CustomerId);

        var licenses = await context.Licenses
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.IssuedAt)
            .ToListAsync(cancellationToken);

        var productIds = licenses.Select(l => l.ProductId.Value).Distinct().ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id.Value))
            .Select(p => new { Id = p.Id.Value, p.Title })
            .ToListAsync(cancellationToken);

        return licenses.Select(l => new LicenseDto(
            l.Id.Value,
            l.ProductId.Value,
            products.FirstOrDefault(p => p.Id == l.ProductId.Value)?.Title ?? "Unknown",
            l.GrantedToTvUsername,
            l.Type.ToString(),
            l.Status.ToString(),
            l.IssuedAt,
            l.ExpiresAt,
            l.AccessGrantedAt
        )).ToList();
    }
}
