using JuanApp.Models;

namespace JuanApp.ViewModels.Shop;

public class ProductDetailVM
{
    public Product Product { get; set; } = null!;
    public List<Product> RelatedProducts { get; set; } = new List<Product>();

    // For submitting review
    public int Rating { get; set; } = 5;
    public string? Comment { get; set; }
}
