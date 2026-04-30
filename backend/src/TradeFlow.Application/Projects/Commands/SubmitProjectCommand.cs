using FluentValidation;
using MediatR;
using TradeFlow.Application.Common.Interfaces;
using TradeFlow.Domain.Common;
using TradeFlow.Domain.Customers;
using TradeFlow.Domain.Projects;

namespace TradeFlow.Application.Projects.Commands;

public record SubmitProjectCommand(
    Guid CustomerId,
    string StrategyTitle,
    string Market,
    string Timeframe,
    string EntryConditions,
    string ExitConditions,
    string RiskManagement,
    string Indicators,
    string? AdditionalNotes,
    string? StrategyType,
    string? TradingViewChartUrl,
    string? BudgetRange,
    DateTime? DesiredDeadline) : IRequest<Result<Guid>>;

public class SubmitProjectCommandValidator : AbstractValidator<SubmitProjectCommand>
{
    public SubmitProjectCommandValidator()
    {
        RuleFor(x => x.StrategyTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Market).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Timeframe).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EntryConditions).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ExitConditions).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.RiskManagement).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Indicators).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TradingViewChartUrl).MaximumLength(500);
    }
}

public class SubmitProjectCommandHandler(IApplicationDbContext db) : IRequestHandler<SubmitProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitProjectCommand request, CancellationToken ct)
    {
        var project = CustomProjectRequest.Submit(
            CustomerId.From(request.CustomerId),
            request.StrategyTitle,
            request.Market,
            request.Timeframe,
            request.EntryConditions,
            request.ExitConditions,
            request.RiskManagement,
            request.Indicators,
            request.AdditionalNotes,
            request.StrategyType,
            request.TradingViewChartUrl,
            request.BudgetRange,
            request.DesiredDeadline);

        db.CustomProjectRequests.Add(project);
        await db.SaveChangesAsync(ct);
        return Result.Success(project.Id.Value);
    }
}
