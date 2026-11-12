using JuanApp.Models;
using JuanApp.Models.Enums;

namespace JuanApp.Areas.Admin.ViewModels.OrderVM;

public class OrderListVM
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}

public class OrderDetailAdminVM
{
    public Order Order { get; set; } = null!;
}
