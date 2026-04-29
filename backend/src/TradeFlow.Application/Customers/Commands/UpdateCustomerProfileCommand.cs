using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Customers.Commands;

public record UpdateCustomerProfileCommand(Guid CustomerId, string? DisplayName, string? TradingViewUsername) : IRequest<Result>;

public class UpdateCustomerProfileCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateCustomerProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerProfileCommand request, CancellationToken ct)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == CustomerId.From(request.CustomerId), ct);
        if (customer is null) return Result.Failure("Customer not found.");

        customer.UpdateProfile(request.DisplayName, request.TradingViewUsername);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
