using Microsoft.EntityFrameworkCore;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Services.Interfaces;

namespace JuanApp.Services.Implements;

public class SubscriberService : ISubscriberService
{
    private readonly JuanDbContext _context;
    private readonly IEmailService _emailService;

    public SubscriberService(JuanDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<(bool Success, string Message)> subscribeAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Please enter a valid email address.");

        email = email.Trim().ToLower();

        var existing = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
        if (existing != null)
        {
            if (existing.IsActive)
                return (false, "You are already subscribed to our newsletter!");

            existing.IsActive = true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, "Welcome back! Your newsletter subscription has been reactivated.");
        }

        var subscriber = new Subscriber
        {
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Subscribers.AddAsync(subscriber);
        await _context.SaveChangesAsync();

        // Send welcome email
        try
        {
            await _emailService.SendEmailAsync(
                email,
                "Welcome to Juan Shoes Newsletter!",
                "<div style='font-family:Arial;padding:20px;text-align:center;'><h2>Thank you for subscribing!</h2><p>You'll be the first to know about new shoes releases, exclusive sales, and member discounts.</p></div>",
                true
            );
        }
        catch { }

        return (true, "Thank you for subscribing! Check your inbox for updates.");
    }

    public async Task<List<Subscriber>> getAllSubscribersAsync()
    {
        return await _context.Subscribers
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }
}
