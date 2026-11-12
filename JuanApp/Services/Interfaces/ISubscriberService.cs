using JuanApp.Models;

namespace JuanApp.Services.Interfaces;

public interface ISubscriberService
{
    Task<(bool Success, string Message)> subscribeAsync(string email);
    Task<List<Subscriber>> getAllSubscribersAsync();
}
