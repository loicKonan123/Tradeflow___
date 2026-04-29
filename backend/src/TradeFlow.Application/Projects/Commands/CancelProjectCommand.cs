using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Commands;

public record CancelProjectCommand(Guid ProjectId) : IRequest<Result>;

public class CancelProjectCommandHandler(IApplicationDbContext db) : IRequestHandler<CancelProjectCommand, Result>
{
    public async Task<Result> Handle(CancelProjectCommand request, CancellationToken ct)
    {
        var project = await db.CustomProjectRequests
            .FirstOrDefaultAsync(p => p.Id == CustomProjectRequestId.From(request.ProjectId), ct);
        if (project is null) return Result.Failure("Project not found.");

        var result = project.Cancel();
        if (result.IsFailure) return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
