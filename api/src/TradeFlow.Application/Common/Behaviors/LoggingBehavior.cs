using MediatR;
using Microsoft.Extensions.Logging;

namespace TradeFlow.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("TradeFlow Request: {Name}", requestName);

        var response = await next(cancellationToken);

        logger.LogInformation("TradeFlow Response: {Name} completed", requestName);
        return response;
    }
}
