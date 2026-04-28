using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Licensing.Commands;

public record GrantLicenseAccessCommand(Guid LicenseId) : IRequest<Result>;

public class GrantLicenseAccessCommandHandler(IApplicationDbContext context, IEmailService emailService)
    : IRequestHandler<GrantLicenseAccessCommand, Result>
{
    public async Task<Result> Handle(GrantLicenseAccessCommand request, CancellationToken cancellationToken)
    {
        var license = await context.Licenses
            .FirstOrDefaultAsync(l => l.Id.Value == request.LicenseId, cancellationToken);

        if (license is null)
            return Result.Failure("License not found.");

        var result = license.MarkAccessGranted();
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        // Email sent by domain event handler (LicenseAccessGrantedEvent)
        return Result.Success();
    }
}
