using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Infrastructure.Services;

public class OrderNumberService(ApplicationDbContext context) : IOrderNumberService
{
    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await context.Orders
            .CountAsync(o => o.CreatedAt.Year == year, cancellationToken);

        return $"TF-{year}-{(count + 1):D4}";
    }
}
