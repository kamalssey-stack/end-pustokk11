namespace JuanApp.ViewModels.Basket;

public class BasketItemVM
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Count { get; set; }
    public decimal SubTotal => Price * Count;
}

public class BasketVM
{
    public List<BasketItemVM> Items { get; set; } = new List<BasketItemVM>();
    public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    public int TotalCount => Items.Sum(i => i.Count);
}
