namespace TradeFlow.Application.Common.Interfaces;

public interface IOrderNumberService
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
