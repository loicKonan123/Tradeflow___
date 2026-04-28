using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;

namespace TradeFlow.Application.Customers.Commands;

public record SyncCustomerCommand(string FirebaseUid, string Email, string? DisplayName) : IRequest<Result<CustomerDto>>;

public record CustomerDto(Guid Id, string Email, string? DisplayName, string Role);

public class SyncCustomerCommandHandler(IApplicationDbContext context) : IRequestHandler<SyncCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(SyncCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .FirstOrDefaultAsync(c => c.FirebaseUid == request.FirebaseUid, cancellationToken);

        if (customer is null)
        {
            customer = Customer.Create(request.FirebaseUid, request.Email, request.DisplayName);
            context.Customers.Add(customer);
        }
        else if (customer.Email != request.Email || customer.DisplayName != request.DisplayName)
        {
            customer.UpdateProfile(request.DisplayName, customer.CountryCode, customer.TradingViewUsername);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CustomerDto(
            customer.Id.Value,
            customer.Email,
            customer.DisplayName,
            customer.Role.ToString()
        ));
    }
}
