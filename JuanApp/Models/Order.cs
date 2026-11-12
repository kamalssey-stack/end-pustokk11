using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JuanApp.Models.Enums;

namespace JuanApp.Models;

public class Order : BaseEntity
{
    [Required, StringLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(500)]
    public string? OrderNote { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = "CashOnDelivery";

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
