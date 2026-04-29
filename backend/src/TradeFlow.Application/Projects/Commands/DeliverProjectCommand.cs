using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Commands;

public record DeliverProjectCommand(Guid ProjectId, Stream FileStream, string FileName, string ContentType, string? DeliveryNotes) : IRequest<Result>;

public class DeliverProjectCommandHandler(IApplicationDbContext db, IStorageService storage)
    : IRequestHandler<DeliverProjectCommand, Result>
{
    public async Task<Result> Handle(DeliverProjectCommand request, CancellationToken ct)
    {
        var project = await db.CustomProjectRequests
            .FirstOrDefaultAsync(p => p.Id == CustomProjectRequestId.From(request.ProjectId), ct);
        if (project is null) return Result.Failure("Project not found.");

        var fileKey = $"projects/{request.ProjectId}/{request.FileName}";
        await storage.UploadFileAsync(request.FileStream, fileKey, request.ContentType, ct);

        var result = project.Deliver(fileKey, request.DeliveryNotes);
        if (result.IsFailure) return result;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
