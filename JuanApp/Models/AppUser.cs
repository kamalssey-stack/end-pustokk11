using Microsoft.AspNetCore.Identity;

namespace JuanApp.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
