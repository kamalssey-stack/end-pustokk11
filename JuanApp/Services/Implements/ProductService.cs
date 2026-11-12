using Microsoft.EntityFrameworkCore;
using JuanApp.Areas.Admin.ViewModels.ProductVM;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Services.Interfaces;
using JuanApp.ViewModels.Common;

namespace JuanApp.Services.Implements;

public class ProductService : IProductService
{
    private readonly JuanDbContext _context;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;

    public ProductService(JuanDbContext context, IFileService fileService, IEmailService emailService)
    {
        _context = context;
        _fileService = fileService;
        _emailService = emailService;
    }

    public async Task<List<ProductListVM>> getAllAdminAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .OrderByDescending(p => p.Id)
            .Select(p => new ProductListVM
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                DiscountPercent = p.DiscountPercent,
                DiscountPrice = p.DiscountPrice,
                StockCount = p.StockCount,
                CategoryName = p.Category != null ? p.Category.Name : "Uncategorized",
                MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain) != null
                    ? p.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                    : (p.Images.FirstOrDefault() != null ? p.Images.FirstOrDefault()!.ImageUrl : "product-1.jpg"),
                IsFeatured = p.IsFeatured,
                IsNew = p.IsNew,
                IsBestSeller = p.IsBestSeller
            })
            .ToListAsync();
    }

    public async Task<PaginationVM<Product>> getAllAsync(int page = 1, int pageSize = 9, int? categoryId = null, string? search = null, string? sortBy = null)
    {
        if (page < 1) page = 1;

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .AsQueryable();

        // Filter by Category
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Search by keyword
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)) || p.SKU.Contains(search));
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price_low" => query.OrderBy(p => p.Price),
            "price_high" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "newest" => query.OrderByDescending(p => p.Id),
            _ => query.OrderByDescending(p => p.Id)
        };

        int totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginationVM<Product>(items, totalCount, page, pageSize);
    }

    public async Task<Product?> getOneAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductDetailAdminVM?> getDetailAdminAsync(int id)
    {
        var product = await getOneAsync(id);
        if (product == null) return null;

        return new ProductDetailAdminVM { Product = product };
    }

    public async Task<ProductUpdateVM?> getForUpdateAsync(int id)
    {
        var product = await getOneAsync(id);
        if (product == null) return null;

        return new ProductUpdateVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price,
            DiscountPercent = product.DiscountPercent,
            StockCount = product.StockCount,
            CategoryId = product.CategoryId,
            IsFeatured = product.IsFeatured,
            IsNew = product.IsNew,
            IsBestSeller = product.IsBestSeller,
            CurrentMainImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl,
            CurrentHoverImageUrl = product.Images.FirstOrDefault(i => i.IsHover)?.ImageUrl,
            CurrentImages = product.Images.ToList()
        };
    }

    public async Task<Product> createAsync(ProductCreateVM model, string baseUrl)
    {
        var product = new Product
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            SKU = model.SKU.Trim(),
            Price = model.Price,
            DiscountPercent = model.DiscountPercent,
            StockCount = model.StockCount,
            CategoryId = model.CategoryId,
            IsFeatured = model.IsFeatured,
            IsNew = model.IsNew,
            IsBestSeller = model.IsBestSeller,
            CreatedAt = DateTime.UtcNow
        };

        // Main image
        if (model.MainImage != null)
        {
            string mainImgName = await _fileService.UploadAsync(model.MainImage, "assets/img/product");
            product.Images.Add(new ProductImage
            {
                ImageUrl = mainImgName,
                IsMain = true,
                IsHover = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Hover image
        if (model.HoverImage != null)
        {
            string hoverImgName = await _fileService.UploadAsync(model.HoverImage, "assets/img/product");
            product.Images.Add(new ProductImage
            {
                ImageUrl = hoverImgName,
                IsMain = false,
                IsHover = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Additional images
        if (model.AdditionalImages != null && model.AdditionalImages.Any())
        {
            foreach (var file in model.AdditionalImages)
            {
                if (file.Length > 0)
                {
                    string addImg = await _fileService.UploadAsync(file, "assets/img/product");
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = addImg,
                        IsMain = false,
                        IsHover = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // 9. Requirement: When a product is created, send email to all active subscribers with link to detail
        try
        {
            var subscribers = await _context.Subscribers
                .Where(s => s.IsActive)
                .Select(s => s.Email)
                .ToListAsync();

            if (subscribers.Any())
            {
                string productDetailUrl = $"{baseUrl.TrimEnd('/')}/Shop/Detail/{product.Id}";
                await _emailService.SendProductNotificationToSubscribersAsync(product, productDetailUrl, subscribers);
            }
        }
        catch
        {
            // Logging handles in email service
        }

        return product;
    }

    public async Task<bool> updateAsync(ProductUpdateVM model)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (product == null) return false;

        product.Name = model.Name.Trim();
        product.Description = model.Description?.Trim();
        product.SKU = model.SKU.Trim();
        product.Price = model.Price;
        product.DiscountPercent = model.DiscountPercent;
        product.StockCount = model.StockCount;
        product.CategoryId = model.CategoryId;
        product.IsFeatured = model.IsFeatured;
        product.IsNew = model.IsNew;
        product.IsBestSeller = model.IsBestSeller;
        product.UpdatedAt = DateTime.UtcNow;

        if (model.MainImage != null)
        {
            var existingMain = product.Images.FirstOrDefault(i => i.IsMain);
            if (existingMain != null)
            {
                _fileService.Delete("assets/img/product", existingMain.ImageUrl);
                product.Images.Remove(existingMain);
            }

            string newMain = await _fileService.UploadAsync(model.MainImage, "assets/img/product");
            product.Images.Add(new ProductImage
            {
                ImageUrl = newMain,
                IsMain = true,
                IsHover = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (model.HoverImage != null)
        {
            var existingHover = product.Images.FirstOrDefault(i => i.IsHover);
            if (existingHover != null)
            {
                _fileService.Delete("assets/img/product", existingHover.ImageUrl);
                product.Images.Remove(existingHover);
            }

            string newHover = await _fileService.UploadAsync(model.HoverImage, "assets/img/product");
            product.Images.Add(new ProductImage
            {
                ImageUrl = newHover,
                IsMain = false,
                IsHover = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (model.AdditionalImages != null && model.AdditionalImages.Any())
        {
            foreach (var file in model.AdditionalImages)
            {
                if (file.Length > 0)
                {
                    string addImg = await _fileService.UploadAsync(file, "assets/img/product");
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = addImg,
                        IsMain = false,
                        IsHover = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> getFeaturedAsync(int count = 8)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> getNewArrivalsAsync(int count = 8)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Where(p => p.IsNew)
            .OrderByDescending(p => p.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> getBestSellersAsync(int count = 8)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Where(p => p.IsBestSeller)
            .OrderByDescending(p => p.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> getRelatedProductsAsync(int categoryId, int excludeProductId, int count = 4)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId)
            .OrderByDescending(p => p.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task addRatingAsync(int productId, int rating, string? comment, string? userId, string userName)
    {
        var productRating = new ProductRating
        {
            ProductId = productId,
            Rating = Math.Clamp(rating, 1, 5),
            Comment = comment?.Trim(),
            AppUserId = userId,
            UserName = string.IsNullOrWhiteSpace(userName) ? "Customer" : userName.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.ProductRatings.AddAsync(productRating);
        await _context.SaveChangesAsync();
    }
}
