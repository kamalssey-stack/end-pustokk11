using JuanApp.Areas.Admin.ViewModels.OrderVM;
using JuanApp.Models;
using JuanApp.Models.Enums;
using JuanApp.ViewModels.Checkout;

namespace JuanApp.Services.Interfaces;

public interface IOrderService
{
    Task<Order> createOrderAsync(CheckoutVM model, string? userId);
    Task<List<OrderListVM>> getAllOrdersAdminAsync();
    Task<Order?> getOrderByIdAsync(int id);
    Task<List<Order>> getUserOrdersAsync(string userId);
    Task<bool> updateOrderStatusAsync(int id, OrderStatus status);
}
