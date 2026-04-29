using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Licensing.Queries;

public record LicenseDto(Guid Id, Guid ProductId, string ProductTitle, string Status, int DownloadCount, int MaxDownloads, DateTime CreatedAt);

public record GetMyLicensesQuery(Guid CustomerId) : IRequest<IReadOnlyList<LicenseDto>>;

public class GetMyLicensesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetMyLicensesQuery, IReadOnlyList<LicenseDto>>
{
    public async Task<IReadOnlyList<LicenseDto>> Handle(GetMyLicensesQuery request, CancellationToken ct)
        => await db.Licenses
            .Join(db.Products, l => l.ProductId, p => p.Id, (l, p) => new { l, p })
            .Where(x => x.l.CustomerId == CustomerId.From(request.CustomerId))
            .OrderByDescending(x => x.l.CreatedAt)
            .Select(x => new LicenseDto(
                x.l.Id.Value, x.p.Id.Value, x.p.Title,
                x.l.Status.ToString(), x.l.DownloadCount, x.l.MaxDownloads, x.l.CreatedAt))
            .ToListAsync(ct);
}
