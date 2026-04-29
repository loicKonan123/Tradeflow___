using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Licensing;

namespace TradeFlow.Application.Licensing.Commands;

public record RevokeLicenseCommand(Guid LicenseId) : IRequest<Result>;

public class RevokeLicenseCommandHandler(IApplicationDbContext db) : IRequestHandler<RevokeLicenseCommand, Result>
{
    public async Task<Result> Handle(RevokeLicenseCommand request, CancellationToken ct)
    {
        var license = await db.Licenses.FirstOrDefaultAsync(l => l.Id == LicenseId.From(request.LicenseId), ct);
        if (license is null) return Result.Failure("License not found.");

        var result = license.Revoke();
        if (result.IsFailure) return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
