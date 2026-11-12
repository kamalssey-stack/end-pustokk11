using System.ComponentModel.DataAnnotations;
using JuanApp.Models;

namespace JuanApp.Areas.Admin.ViewModels.CategoryVM;

public class CategoryListVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CategoryCreateVM
{
    [Required(ErrorMessage = "Category name is required"), StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class CategoryUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required"), StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class CategoryDetailVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}
