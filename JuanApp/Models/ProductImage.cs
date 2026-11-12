using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models;

public class ProductImage : BaseEntity
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsMain { get; set; } = false;
    public bool IsHover { get; set; } = false;

    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
