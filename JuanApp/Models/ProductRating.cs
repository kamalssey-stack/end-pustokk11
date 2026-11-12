using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models;

public class ProductRating : BaseEntity
{
    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [StringLength(500)]
    public string? Comment { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public string UserName { get; set; } = "Anonymous";
}
