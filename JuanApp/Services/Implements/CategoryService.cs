using Microsoft.EntityFrameworkCore;
using JuanApp.Areas.Admin.ViewModels.CategoryVM;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Services.Interfaces;

namespace JuanApp.Services.Implements;

public class CategoryService : ICategoryService
{
    private readonly JuanDbContext _context;

    public CategoryService(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryListVM>> getAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryListVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count(p => !p.IsDeleted),
                CreatedAt = c.CreatedAt
            })
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<CategoryDetailVM?> getOneAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Images.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new CategoryDetailVM
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            Products = category.Products.ToList()
        };
    }

    public async Task<CategoryUpdateVM?> getForUpdateAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        return new CategoryUpdateVM
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task createAsync(CategoryCreateVM model)
    {
        var category = new Category
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> updateAsync(CategoryUpdateVM model)
    {
        var category = await _context.Categories.FindAsync(model.Id);
        if (category == null) return false;

        category.Name = model.Name.Trim();
        category.Description = model.Description?.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> deleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Category>> getActiveCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
