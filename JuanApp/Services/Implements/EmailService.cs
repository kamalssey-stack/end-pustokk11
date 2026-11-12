using System.Net;
using System.Net.Mail;
using JuanApp.Models;
using JuanApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JuanApp.Services.Implements;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var host = _config["Email:Host"] ?? "smtp.gmail.com";
            var portStr = _config["Email:Port"] ?? "587";
            int.TryParse(portStr, out int port);
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromEmail = _config["Email:From"] ?? username ?? "noreply@juan.com";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning($"[EMAIL SIMULATION] To: {to} | Subject: {subject}");
                return;
            }

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "Juan Shoes Store"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {to}: {ex.Message}");
        }
    }

    public async Task SendProductNotificationToSubscribersAsync(Product product, string productDetailUrl, List<string> subscriberEmails)
    {
        if (subscriberEmails == null || !subscriberEmails.Any()) return;

        string subject = $"🔥 New Arrival at Juan Shoes: {product.Name}!";
        string body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                <h2 style='color: #e3a51e; text-align: center; margin-bottom: 20px;'>New Shoe Collection Arrived!</h2>
                <p>Hello valued subscriber,</p>
                <p>We are thrilled to introduce our newest arrival in store:</p>
                <div style='text-align: center; margin: 25px 0;'>
                    <h3 style='color: #333; margin: 10px 0;'>{product.Name}</h3>
                    <p style='color: #666;'>{product.Description}</p>
                    <p style='font-size: 20px; font-weight: bold; color: #e3a51e;'>Price: ${product.Price:F2}</p>
                    <a href='{productDetailUrl}' style='display: inline-block; background-color: #e3a51e; color: #fff; padding: 12px 28px; text-decoration: none; border-radius: 4px; font-weight: bold; margin-top: 15px;'>View Product Detail & Buy Now</a>
                </div>
                <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;' />
                <p style='font-size: 12px; color: #888; text-align: center;'>You received this email because you subscribed to Juan Shoes Store updates.</p>
            </div>";

        foreach (var email in subscriberEmails)
        {
            await SendEmailAsync(email, subject, body, true);
        }
    }

    public async Task SendOrderConfirmationAsync(Order order)
    {
        string subject = $"✅ Juan Shoes - Order Confirmation #{order.OrderNumber}";
        string itemsHtml = "";
        foreach (var item in order.OrderItems)
        {
            itemsHtml += $"<tr><td style='padding: 8px; border-bottom: 1px solid #eee;'>{item.ProductName}</td><td style='padding: 8px; border-bottom: 1px solid #eee;'>{item.Count}</td><td style='padding: 8px; border-bottom: 1px solid #eee;'>${item.TotalPrice:F2}</td></tr>";
        }

        string body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                <h2 style='color: #28a745; text-align: center;'>Thank You For Your Order!</h2>
                <p>Dear {order.CustomerName},</p>
                <p>Your order <strong>#{order.OrderNumber}</strong> has been received and is now being processed.</p>
                <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                    <thead>
                        <tr style='background-color: #f8f9fa; text-align: left;'>
                            <th style='padding: 8px;'>Product</th>
                            <th style='padding: 8px;'>Quantity</th>
                            <th style='padding: 8px;'>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemsHtml}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan='2' style='padding: 10px; font-weight: bold; text-align: right;'>Grand Total:</td>
                            <td style='padding: 10px; font-weight: bold; color: #e3a51e;'>${order.TotalAmount:F2}</td>
                        </tr>
                    </tfoot>
                </table>
                <p><strong>Delivery Address:</strong> {order.Address}, {order.City}</p>
                <p><strong>Contact Phone:</strong> {order.Phone}</p>
                <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;' />
                <p style='font-size: 12px; color: #888; text-align: center;'>Juan Shoes Store &copy; {DateTime.UtcNow.Year}</p>
            </div>";

        await SendEmailAsync(order.Email, subject, body, true);
    }
}
