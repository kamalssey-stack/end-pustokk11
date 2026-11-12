using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using JuanApp.Models;

namespace JuanApp.Areas.Admin.ViewModels.ProductVM;

public class ProductListVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public int StockCount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
}

public class ProductCreateVM
{
    [Required(ErrorMessage = "Product name is required"), StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "SKU is required"), StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required, Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, 100)]
    public int? DiscountPercent { get; set; }

    [Required, Range(0, 10000)]
    public int StockCount { get; set; } = 10;

    [Required(ErrorMessage = "Please select category")]
    public int CategoryId { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; } = true;
    public bool IsBestSeller { get; set; }

    [Required(ErrorMessage = "Please upload a main product image")]
    public IFormFile MainImage { get; set; } = null!;

    public IFormFile? HoverImage { get; set; }

    public List<IFormFile>? AdditionalImages { get; set; }
}

public class ProductUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required"), StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required, StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required, Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, 100)]
    public int? DiscountPercent { get; set; }

    [Required, Range(0, 10000)]
    public int StockCount { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }

    public string? CurrentMainImageUrl { get; set; }
    public string? CurrentHoverImageUrl { get; set; }
    public List<ProductImage> CurrentImages { get; set; } = new List<ProductImage>();

    public IFormFile? MainImage { get; set; }
    public IFormFile? HoverImage { get; set; }
    public List<IFormFile>? AdditionalImages { get; set; }
}

public class ProductDetailAdminVM
{
    public Product Product { get; set; } = null!;
}
