using System.Security.Claims;
using JuanApp.Services.Interfaces;
using JuanApp.ViewModels.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Controllers;

public class CheckoutController(IBasketService basketService, IOrderService orderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() => Json(new { basket = await basketService.getBasketAsync() });

    [HttpPost]
    public async Task<IActionResult> Index(CheckoutVM model)
    {
        model.Basket = await basketService.getBasketAsync();
        if (!ModelState.IsValid || !model.Basket.Items.Any()) return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage), basket = model.Basket });
        try
        {
            var order = await orderService.createOrderAsync(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(new { order.Id, order.OrderNumber, order.TotalAmount, order.Status });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
