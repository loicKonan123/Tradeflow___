using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Commands;

public record SendQuoteCommand(Guid ProjectId, decimal Price, string Currency, string? AdminNotes) : IRequest<Result>;

public class SendQuoteCommandHandler(IApplicationDbContext db) : IRequestHandler<SendQuoteCommand, Result>
{
    public async Task<Result> Handle(SendQuoteCommand request, CancellationToken ct)
    {
        var project = await db.CustomProjectRequests
            .FirstOrDefaultAsync(p => p.Id == CustomProjectRequestId.From(request.ProjectId), ct);
        if (project is null) return Result.Failure("Project not found.");

        var result = project.SendQuote(request.Price, request.Currency, request.AdminNotes);
        if (result.IsFailure) return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
