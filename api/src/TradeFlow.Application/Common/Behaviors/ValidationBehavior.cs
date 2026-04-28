using FluentValidation;
using MediatR;
using TradeFlow.Domain.Common;

namespace TradeFlow.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));
            return (TResponse)Result.Failure(errors);
        }

        return await next(cancellationToken);
    }
}
