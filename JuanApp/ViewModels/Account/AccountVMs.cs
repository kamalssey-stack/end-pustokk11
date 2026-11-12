using System.ComponentModel.DataAnnotations;

namespace JuanApp.ViewModels.Account;

public class AccountLoginVM
{
    [Required(ErrorMessage = "Username or Email is required")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class AccountRegisterVM
{
    [Required(ErrorMessage = "Full Name is required"), StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required"), StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required"), EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required"), DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required"), DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required"), EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordVM
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class TwoFactorVM
{
    [Required(ErrorMessage = "Verification Code is required")]
    [Display(Name = "Two-Factor Code")]
    public string Code { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public bool RememberMachine { get; set; }
}

public class MyAccountVM
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public List<JuanApp.Models.Order> Orders { get; set; } = new List<JuanApp.Models.Order>();
}
