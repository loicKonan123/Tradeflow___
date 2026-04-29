using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record PublishProductCommand(Guid Id) : IRequest<Result>;

public class PublishProductCommandHandler(IApplicationDbContext db) : IRequestHandler<PublishProductCommand, Result>
{
    public async Task<Result> Handle(PublishProductCommand request, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(request.Id), ct);
        if (product is null) return Result.Failure("Product not found.");

        var result = product.Publish();
        if (result.IsFailure) return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
