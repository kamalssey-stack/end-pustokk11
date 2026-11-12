using Microsoft.EntityFrameworkCore;
using JuanApp.Areas.Admin.ViewModels.OrderVM;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Models.Enums;
using JuanApp.Services.Interfaces;
using JuanApp.ViewModels.Checkout;

namespace JuanApp.Services.Implements;

public class OrderService : IOrderService
{
    private readonly JuanDbContext _context;
    private readonly IBasketService _basketService;
    private readonly IEmailService _emailService;

    public OrderService(JuanDbContext context, IBasketService basketService, IEmailService emailService)
    {
        _context = context;
        _basketService = basketService;
        _emailService = emailService;
    }

    public async Task<Order> createOrderAsync(CheckoutVM model, string? userId)
    {
        var basket = await _basketService.getBasketAsync();
        if (!basket.Items.Any())
        {
            throw new InvalidOperationException("Basket is empty.");
        }

        string orderNumber = "JUAN-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

        var order = new Order
        {
            OrderNumber = orderNumber,
            AppUserId = userId,
            CustomerName = model.CustomerName.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            Address = model.Address.Trim(),
            City = model.City.Trim(),
            PostalCode = model.PostalCode?.Trim(),
            OrderNote = model.OrderNote?.Trim(),
            TotalAmount = basket.TotalAmount,
            Status = OrderStatus.Pending,
            PaymentMethod = "CashOnDelivery",
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in basket.Items)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.Price,
                Count = item.Count,
                TotalPrice = item.SubTotal,
                CreatedAt = DateTime.UtcNow
            });

            // Decrement stock count
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockCount = Math.Max(0, product.StockCount - item.Count);
            }
        }

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // Clear cart
        await _basketService.clearBasketAsync();

        // Send order confirmation email
        try
        {
            await _emailService.SendOrderConfirmationAsync(order);
        }
        catch
        {
            // Handled in EmailService
        }

        return order;
    }

    public async Task<List<OrderListVM>> getAllOrdersAdminAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.Id)
            .Select(o => new OrderListVM
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                Email = o.Email,
                Phone = o.Phone,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ItemCount = o.OrderItems.Sum(oi => oi.Count)
            })
            .ToListAsync();
    }

    public async Task<Order?> getOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
            .Include(o => o.AppUser)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> getUserOrdersAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.AppUserId == userId)
            .OrderByDescending(o => o.Id)
            .ToListAsync();
    }

    public async Task<bool> updateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
