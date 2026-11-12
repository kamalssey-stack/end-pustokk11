using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Services.Interfaces;
using JuanApp.ViewModels.Basket;

namespace JuanApp.Services.Implements;

public class BasketService : IBasketService
{
    private readonly JuanDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CookieBasketName = "JUAN_BASKET";

    public BasketService(JuanDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext Context => _httpContextAccessor.HttpContext!;
    private string? CurrentUserId => Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

    private class LocalBasketItem
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }

    private List<LocalBasketItem> GetCookieBasket()
    {
        if (Context.Request.Cookies.TryGetValue(CookieBasketName, out string? json) && !string.IsNullOrEmpty(json))
        {
            try
            {
                return JsonSerializer.Deserialize<List<LocalBasketItem>>(json) ?? new List<LocalBasketItem>();
            }
            catch
            {
                return new List<LocalBasketItem>();
            }
        }
        return new List<LocalBasketItem>();
    }

    private void SetCookieBasket(List<LocalBasketItem> items)
    {
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true
        };
        Context.Response.Cookies.Append(CookieBasketName, JsonSerializer.Serialize(items), options);
    }

    public async Task<BasketVM> getBasketAsync()
    {
        var basketVM = new BasketVM();
        var userId = CurrentUserId;

        List<LocalBasketItem> basketList;

        if (!string.IsNullOrEmpty(userId))
        {
            // If logged in, load from DB
            var dbItems = await _context.BasketItems
                .Where(b => b.AppUserId == userId)
                .ToListAsync();

            basketList = dbItems.Select(b => new LocalBasketItem { ProductId = b.ProductId, Count = b.Count }).ToList();
        }
        else
        {
            basketList = GetCookieBasket();
        }

        if (basketList.Any())
        {
            var productIds = basketList.Select(b => b.ProductId).ToList();
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in basketList)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    var mainImg = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl
                        ?? product.Images.FirstOrDefault()?.ImageUrl
                        ?? "product-1.jpg";

                    basketVM.Items.Add(new BasketItemVM
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ImageUrl = mainImg,
                        Price = product.DiscountPrice,
                        Count = item.Count
                    });
                }
            }
        }

        return basketVM;
    }

    public async Task addToBasketAsync(int productId, int count = 1)
    {
        if (count < 1) count = 1;
        var userId = CurrentUserId;

        if (!string.IsNullOrEmpty(userId))
        {
            var dbItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.AppUserId == userId && b.ProductId == productId);

            if (dbItem != null)
            {
                dbItem.Count += count;
                dbItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await _context.BasketItems.AddAsync(new BasketItem
                {
                    AppUserId = userId,
                    ProductId = productId,
                    Count = count,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }
        else
        {
            var cookieBasket = GetCookieBasket();
            var item = cookieBasket.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Count += count;
            }
            else
            {
                cookieBasket.Add(new LocalBasketItem { ProductId = productId, Count = count });
            }
            SetCookieBasket(cookieBasket);
        }
    }

    public async Task changeCountAsync(int productId, int count)
    {
        var userId = CurrentUserId;

        if (!string.IsNullOrEmpty(userId))
        {
            var dbItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.AppUserId == userId && b.ProductId == productId);

            if (dbItem != null)
            {
                if (count <= 0)
                {
                    _context.BasketItems.Remove(dbItem);
                }
                else
                {
                    dbItem.Count = count;
                    dbItem.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            var cookieBasket = GetCookieBasket();
            var item = cookieBasket.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (count <= 0)
                {
                    cookieBasket.Remove(item);
                }
                else
                {
                    item.Count = count;
                }
                SetCookieBasket(cookieBasket);
            }
        }
    }

    public async Task removeFromBasketAsync(int productId)
    {
        await changeCountAsync(productId, 0);
    }

    public async Task clearBasketAsync()
    {
        var userId = CurrentUserId;

        if (!string.IsNullOrEmpty(userId))
        {
            var dbItems = await _context.BasketItems
                .Where(b => b.AppUserId == userId)
                .ToListAsync();

            if (dbItems.Any())
            {
                _context.BasketItems.RemoveRange(dbItems);
                await _context.SaveChangesAsync();
            }
        }

        Context.Response.Cookies.Delete(CookieBasketName);
    }
}
