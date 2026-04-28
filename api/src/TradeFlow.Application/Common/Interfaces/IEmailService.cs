namespace TradeFlow.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string to, string orderNumber, CancellationToken cancellationToken = default);
    Task SendLicenseGrantedAsync(string to, string productTitle, string tvUsername, CancellationToken cancellationToken = default);
    Task SendWelcomeAsync(string to, string displayName, CancellationToken cancellationToken = default);
}
