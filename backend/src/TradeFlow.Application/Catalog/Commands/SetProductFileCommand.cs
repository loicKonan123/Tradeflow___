using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record SetProductFileCommand(Guid ProductId, Stream FileStream, string FileName, string ContentType) : IRequest<Result<string>>;

public class SetProductFileCommandHandler(IApplicationDbContext db, IStorageService storage)
    : IRequestHandler<SetProductFileCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SetProductFileCommand request, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(request.ProductId), ct);
        if (product is null) return Result.Failure<string>("Product not found.");

        var fileKey = $"products/{request.ProductId}/{request.FileName}";
        var url = await storage.UploadFileAsync(request.FileStream, fileKey, request.ContentType, ct);

        product.SetFileUrl(fileKey);
        await db.SaveChangesAsync(ct);
        return Result.Success(url);
    }
}
