using FluentValidation;
using MediatR;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Commands;

public record SubmitProjectCommand(
    Guid CustomerId,
    string Market,
    string Timeframe,
    string EntryConditions,
    string ExitConditions,
    string RiskManagement,
    string? AdditionalNotes) : IRequest<Result<Guid>>;

public class SubmitProjectCommandValidator : AbstractValidator<SubmitProjectCommand>
{
    public SubmitProjectCommandValidator()
    {
        RuleFor(x => x.Market).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Timeframe).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EntryConditions).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ExitConditions).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.RiskManagement).NotEmpty().MaximumLength(1000);
    }
}

public class SubmitProjectCommandHandler(IApplicationDbContext db) : IRequestHandler<SubmitProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitProjectCommand request, CancellationToken ct)
    {
        var project = CustomProjectRequest.Submit(
            CustomerId.From(request.CustomerId),
            request.Market,
            request.Timeframe,
            request.EntryConditions,
            request.ExitConditions,
            request.RiskManagement,
            request.AdditionalNotes);

        db.CustomProjectRequests.Add(project);
        await db.SaveChangesAsync(ct);
        return Result.Success(project.Id.Value);
    }
}
