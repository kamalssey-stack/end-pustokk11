using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JuanApp.Areas.Admin.ViewModels.SliderVM;

public class SliderCreateVM
{
    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }
    public string? Description { get; set; }

    public string? ButtonText { get; set; } = "Shop Now";
    public string? ButtonLink { get; set; } = "/Shop";

    public int OrderNumber { get; set; } = 1;

    [Required(ErrorMessage = "Please upload slider image")]
    public IFormFile Image { get; set; } = null!;
}

public class SliderUpdateVM
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }
    public string? Description { get; set; }

    public string? ButtonText { get; set; } = "Shop Now";
    public string? ButtonLink { get; set; } = "/Shop";

    public int OrderNumber { get; set; } = 1;

    public string? CurrentImageUrl { get; set; }
    public IFormFile? Image { get; set; }
}
