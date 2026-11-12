using JuanApp.Areas.Admin.ViewModels.ProductVM;
using JuanApp.Models;
using JuanApp.ViewModels.Common;

namespace JuanApp.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductListVM>> getAllAdminAsync();
    Task<PaginationVM<Product>> getAllAsync(int page = 1, int pageSize = 9, int? categoryId = null, string? search = null, string? sortBy = null);
    Task<Product?> getOneAsync(int id);
    Task<ProductDetailAdminVM?> getDetailAdminAsync(int id);
    Task<ProductUpdateVM?> getForUpdateAsync(int id);
    Task<Product> createAsync(ProductCreateVM model, string baseUrl);
    Task<bool> updateAsync(ProductUpdateVM model);
    Task<bool> deleteAsync(int id);
    Task<List<Product>> getFeaturedAsync(int count = 8);
    Task<List<Product>> getNewArrivalsAsync(int count = 8);
    Task<List<Product>> getBestSellersAsync(int count = 8);
    Task<List<Product>> getRelatedProductsAsync(int categoryId, int excludeProductId, int count = 4);
    Task addRatingAsync(int productId, int rating, string? comment, string? userId, string userName);
}
