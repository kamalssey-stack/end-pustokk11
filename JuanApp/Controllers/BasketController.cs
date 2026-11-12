using JuanApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Controllers;

public class BasketController(IBasketService basketService, IProductService products) : Controller
{
    public async Task<IActionResult> Index() => Json(await basketService.getBasketAsync());

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int count = 1)
    {
        if (count < 1 || count > 50) return BadRequest(new { message = "Count must be between 1 and 50." });
        if (await products.getOneAsync(productId) is null) return NotFound();
        await basketService.addToBasketAsync(productId, count);
        return Ok(await basketService.getBasketAsync());
    }

    [HttpPost]
    public async Task<IActionResult> ChangeCount(int productId, int count)
    {
        if (count > 50) return BadRequest(new { message = "Maximum quantity is 50." });
        await basketService.changeCountAsync(productId, count);
        return Ok(await basketService.getBasketAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        await basketService.removeFromBasketAsync(productId);
        return Ok(await basketService.getBasketAsync());
    }
}
