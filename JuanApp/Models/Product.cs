using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JuanApp.Models;

public class Product : BaseEntity
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int? DiscountPercent { get; set; }

    [NotMapped]
    public decimal DiscountPrice => DiscountPercent.HasValue && DiscountPercent.Value > 0
        ? Math.Round(Price * (100 - DiscountPercent.Value) / 100m, 2)
        : Price;

    public int StockCount { get; set; } = 10;

    public bool IsFeatured { get; set; } = false;
    public bool IsNew { get; set; } = true;
    public bool IsBestSeller { get; set; } = false;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
}
