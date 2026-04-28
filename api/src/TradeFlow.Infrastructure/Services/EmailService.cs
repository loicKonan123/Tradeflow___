using Microsoft.Extensions.Logging;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

// Stub — sera implémenté avec Brevo SDK en Sprint 4
public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendOrderConfirmationAsync(string to, string orderNumber, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("TODO: Send order confirmation to {Email} for order {OrderNumber}", to, orderNumber);
        return Task.CompletedTask;
    }

    public Task SendLicenseGrantedAsync(string to, string productTitle, string tvUsername, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("TODO: Send license granted to {Email} for product {Product}", to, productTitle);
        return Task.CompletedTask;
    }

    public Task SendWelcomeAsync(string to, string displayName, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("TODO: Send welcome email to {Email}", to);
        return Task.CompletedTask;
    }
}
