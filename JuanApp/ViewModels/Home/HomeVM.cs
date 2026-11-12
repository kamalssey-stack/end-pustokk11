using JuanApp.Models;

namespace JuanApp.ViewModels.Home;

public class HomeVM
{
    public List<Slider> Sliders { get; set; } = new List<Slider>();
    public List<Product> FeaturedProducts { get; set; } = new List<Product>();
    public List<Product> NewProducts { get; set; } = new List<Product>();
    public List<Product> BestSellerProducts { get; set; } = new List<Product>();
}
