using JuanApp.ViewModels.Basket;

namespace JuanApp.Services.Interfaces;

public interface IBasketService
{
    Task<BasketVM> getBasketAsync();
    Task addToBasketAsync(int productId, int count = 1);
    Task changeCountAsync(int productId, int count);
    Task removeFromBasketAsync(int productId);
    Task clearBasketAsync();
}
