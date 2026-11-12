using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models;

public class Slider : BaseEntity
{
    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Subtitle { get; set; }

    public string? Description { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public string? ButtonText { get; set; } = "Shop Now";
    public string? ButtonLink { get; set; } = "/Shop";

    public int OrderNumber { get; set; } = 1;
}
