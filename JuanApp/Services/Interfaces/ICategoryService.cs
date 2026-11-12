using JuanApp.Areas.Admin.ViewModels.CategoryVM;
using JuanApp.Models;

namespace JuanApp.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryListVM>> getAllAsync();
    Task<CategoryDetailVM?> getOneAsync(int id);
    Task<CategoryUpdateVM?> getForUpdateAsync(int id);
    Task createAsync(CategoryCreateVM model);
    Task<bool> updateAsync(CategoryUpdateVM model);
    Task<bool> deleteAsync(int id);
    Task<List<Category>> getActiveCategoriesAsync();
}
