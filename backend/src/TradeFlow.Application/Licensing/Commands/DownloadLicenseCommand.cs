using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Licensing;

namespace TradeFlow.Application.Licensing.Commands;

public record DownloadLicenseCommand(Guid LicenseId, Guid CustomerId) : IRequest<Result<string>>;

public class DownloadLicenseCommandHandler(IApplicationDbContext db, IStorageService storage)
    : IRequestHandler<DownloadLicenseCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DownloadLicenseCommand request, CancellationToken ct)
    {
        var license = await db.Licenses
            .Include(l => l.ProductId)
            .FirstOrDefaultAsync(l => l.Id == LicenseId.From(request.LicenseId)
                                   && l.CustomerId == CustomerId.From(request.CustomerId), ct);

        if (license is null) return Result.Failure<string>("License not found.");

        var downloadResult = license.RecordDownload();
        if (downloadResult.IsFailure) return Result.Failure<string>(downloadResult.Error!);

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == license.ProductId, ct);
        if (product is null || string.IsNullOrEmpty(product.FileUrl))
            return Result.Failure<string>("File not available.");

        var signedUrl = await storage.GenerateSignedDownloadUrlAsync(product.FileUrl, TimeSpan.FromHours(1), ct);
        await db.SaveChangesAsync(ct);
        return Result.Success(signedUrl);
    }
}
