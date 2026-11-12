using JuanApp.Models;
using JuanApp.ViewModels.Common;

namespace JuanApp.ViewModels.Shop;

public class ShopVM
{
    public PaginationVM<Product> Products { get; set; } = null!;
    public List<Category> Categories { get; set; } = new List<Category>();

    public int? SelectedCategoryId { get; set; }
    public string? Search { get; set; }
    public string? SortBy { get; set; }
}
