using Microsoft.EntityFrameworkCore;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Licensing;
using TradeFlow.Domain.Orders;

namespace TradeFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Order> Orders { get; }
    DbSet<License> Licenses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
