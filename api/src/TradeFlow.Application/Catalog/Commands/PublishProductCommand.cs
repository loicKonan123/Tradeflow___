using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Catalog.Commands;

public record PublishProductCommand(Guid ProductId) : IRequest<Result>;

public class PublishProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<PublishProductCommand, Result>
{
    public async Task<Result> Handle(PublishProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id.Value == request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        var result = product.Publish();
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
