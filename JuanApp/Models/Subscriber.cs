using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models;

public class Subscriber : BaseEntity
{
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
