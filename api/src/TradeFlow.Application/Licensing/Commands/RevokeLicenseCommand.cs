using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Licensing.Commands;

public record RevokeLicenseCommand(Guid LicenseId, string Reason) : IRequest<Result>;

public class RevokeLicenseCommandValidator : AbstractValidator<RevokeLicenseCommand>
{
    public RevokeLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class RevokeLicenseCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RevokeLicenseCommand, Result>
{
    public async Task<Result> Handle(RevokeLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await context.Licenses
            .FirstOrDefaultAsync(l => l.Id.Value == request.LicenseId, cancellationToken);

        if (license is null)
            return Result.Failure("License not found.");

        var result = license.Revoke(request.Reason);
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
