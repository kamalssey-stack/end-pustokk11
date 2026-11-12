using JuanApp.Models;

namespace JuanApp.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendProductNotificationToSubscribersAsync(Product product, string productDetailUrl, List<string> subscriberEmails);
    Task SendOrderConfirmationAsync(Order order);
}
