using JuanApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Controllers;

public class ShopController(IProductService products, ICategoryService categories) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string? search = null, string? sortBy = null)
    {
        var result = await products.getAllAsync(Math.Max(page, 1), 12, categoryId, search, sortBy);
        return Json(new { result.Items, result.CurrentPage, result.TotalPages, result.TotalCount, categories = await categories.getActiveCategoriesAsync(), categoryId, search, sortBy });
    }

    public async Task<IActionResult> Detail(int id)
    {
        var product = await products.getOneAsync(id);
        if (product is null) return NotFound();
        var related = product.CategoryId > 0 ? await products.getRelatedProductsAsync(product.CategoryId, product.Id) : [];
        return Json(new { product, related });
    }

    [HttpPost]
    public async Task<IActionResult> Rate(int productId, int rating, string? comment)
    {
        if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();
        if (rating is < 1 or > 5) return BadRequest(new { message = "Rating must be from 1 to 5." });
        await products.addRatingAsync(productId, rating, comment, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, User.Identity?.Name ?? "Customer");
        return Ok(new { success = true });
    }
}
