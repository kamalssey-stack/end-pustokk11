using System.ComponentModel.DataAnnotations;
using JuanApp.ViewModels.Basket;

namespace JuanApp.ViewModels.Checkout;

public class CheckoutVM
{
    [Required(ErrorMessage = "Full Name is required"), StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required"), EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required"), Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required"), StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required"), StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(500)]
    public string? OrderNote { get; set; }

    public BasketVM Basket { get; set; } = new BasketVM();
}
