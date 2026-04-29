using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Customers.Commands;

public record SyncCustomerCommand(string FirebaseUid, string Email, string? DisplayName) : IRequest<Result<Guid>>;

public class SyncCustomerCommandHandler(IApplicationDbContext db) : IRequestHandler<SyncCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SyncCustomerCommand request, CancellationToken ct)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.FirebaseUid == request.FirebaseUid, ct);

        if (customer is null)
        {
            customer = Customer.Create(request.FirebaseUid, request.Email, request.DisplayName);
            db.Customers.Add(customer);
        }

        await db.SaveChangesAsync(ct);
        return Result.Success(customer.Id.Value);
    }
}
