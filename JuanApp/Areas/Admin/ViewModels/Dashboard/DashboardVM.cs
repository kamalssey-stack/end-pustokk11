using JuanApp.Models;

namespace JuanApp.Areas.Admin.ViewModels.Dashboard;

public class DashboardVM
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSalesAmount { get; set; }
    public int TotalSubscribers { get; set; }
    public int TotalUsers { get; set; }

    public List<Order> RecentOrders { get; set; } = new List<Order>();
}
