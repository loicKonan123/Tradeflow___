using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record ArchiveProductCommand(Guid ProductId) : IRequest<Result>;

public class ArchiveProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ArchiveProductCommand, Result>
{
    public async Task<Result> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id.Value == request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        var result = product.Archive();
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
