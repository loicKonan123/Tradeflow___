using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Infrastructure.Services;

public class OrderNumberService(ApplicationDbContext db) : IOrderNumberService
{
    public async Task<string> GenerateAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await db.Orders.CountAsync(ct) + 1;
        return $"TF-{year}-{count:D4}";
    }
}
