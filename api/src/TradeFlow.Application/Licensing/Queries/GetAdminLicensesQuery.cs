using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Licensing;
using TradeFlow.Application.Orders.Queries;

namespace TradeFlow.Application.Licensing.Queries;

public record GetAdminLicensesQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<LicenseAdminDto>>;

public record LicenseAdminDto(
    Guid Id,
    Guid CustomerId,
    Guid ProductId,
    string TvUsername,
    string Type,
    string Status,
    DateTime IssuedAt,
    DateTime? AccessGrantedAt,
    DateTime? ExpiresAt
);

public class GetAdminLicensesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAdminLicensesQuery, PagedResult<LicenseAdminDto>>
{
    public async Task<PagedResult<LicenseAdminDto>> Handle(GetAdminLicensesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Licenses.AsQueryable();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<LicenseStatus>(request.Status, true, out var status))
            query = query.Where(l => l.Status == status);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.IssuedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LicenseAdminDto(
                l.Id.Value,
                l.CustomerId.Value,
                l.ProductId.Value,
                l.GrantedToTvUsername,
                l.Type.ToString(),
                l.Status.ToString(),
                l.IssuedAt,
                l.AccessGrantedAt,
                l.ExpiresAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<LicenseAdminDto>(items, total, request.Page, request.PageSize);
    }
}
