using Microsoft.EntityFrameworkCore;
using TradeFlow.Domain.Catalog;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Licensing;
using TradeFlow.Domain.Orders;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<ProductMedia> ProductMedias { get; }
    DbSet<BacktestReport> BacktestReports { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<License> Licenses { get; }
    DbSet<CustomProjectRequest> CustomProjectRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
