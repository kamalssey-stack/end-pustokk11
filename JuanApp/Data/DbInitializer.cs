using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JuanApp.Models;

namespace JuanApp.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(JuanDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // 1. Roles Seed
        string[] roles = new[] { "Admin", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Default Admin User
        var adminEmail = "admin@juan.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true,
                City = "Baku",
                Address = "Nizami str. 45"
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Default Member User
        var memberEmail = "member@juan.com";
        var memberUser = await userManager.FindByEmailAsync(memberEmail);
        if (memberUser == null)
        {
            memberUser = new AppUser
            {
                UserName = "member",
                Email = memberEmail,
                FullName = "Ali Mammadov",
                EmailConfirmed = true,
                City = "Baku",
                Address = "28 May str. 12"
            };
            var result = await userManager.CreateAsync(memberUser, "Member123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(memberUser, "Member");
            }
        }

        // 4. Categories Seed (Seed Entity 1)
        if (!await context.Categories.IgnoreQueryFilters().AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Men Shoes", Description = "Premium stylish men's footwear collection" },
                new Category { Name = "Women Shoes", Description = "Trendy and comfortable shoes for women" },
                new Category { Name = "Running & Sports", Description = "High performance athletic and running sneakers" },
                new Category { Name = "Casual Footwear", Description = "Everyday comfort sneakers and loafers" },
                new Category { Name = "Leather Boots", Description = "Durable genuine leather winter and autumn boots" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 5. Sliders Seed
        if (!await context.Sliders.IgnoreQueryFilters().AnyAsync())
        {
            var sliders = new List<Slider>
            {
                new Slider
                {
                    Title = "Top Selling Shoes",
                    Subtitle = "New Season Collection",
                    Description = "Discover the newest arrivals of premium sports and leather shoes with high durability.",
                    ImageUrl = "slider-1.jpg",
                    ButtonText = "Shop Now",
                    ButtonLink = "/Shop",
                    OrderNumber = 1
                },
                new Slider
                {
                    Title = "Exclusive Sports Sneakers",
                    Subtitle = "Save Up To 30% Off",
                    Description = "Engineered for maximum speed, athletic comfort and supreme cushioning on every step.",
                    ImageUrl = "slider-2.jpg",
                    ButtonText = "Explore Collection",
                    ButtonLink = "/Shop",
                    OrderNumber = 2
                }
            };

            await context.Sliders.AddRangeAsync(sliders);
            await context.SaveChangesAsync();
        }

        // 6. Products Seed (Seed Entity 2)
        if (!await context.Products.IgnoreQueryFilters().AnyAsync())
        {
            var menCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Men Shoes");
            var womenCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Women Shoes");
            var sportsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Running & Sports");
            var casualCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Casual Footwear");

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Air Running Flex Pro",
                    Description = "Featherlight breathable knit sneakers designed for marathon runners and casual daily strolls.",
                    SKU = "JUAN-RUN-01",
                    Price = 129.99m,
                    DiscountPercent = 15,
                    StockCount = 25,
                    IsFeatured = true,
                    IsNew = true,
                    IsBestSeller = true,
                    CategoryId = sportsCategory?.Id ?? 1,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-1.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-2.jpg", IsMain = false, IsHover = true },
                        new ProductImage { ImageUrl = "product-details-img1.jpg", IsMain = false, IsHover = false }
                    }
                },
                new Product
                {
                    Name = "Urban Street Classic Leather",
                    Description = "Minimalist white and dark accents genuine calfskin leather sneakers for modern street fashion.",
                    SKU = "JUAN-URB-02",
                    Price = 99.50m,
                    DiscountPercent = null,
                    StockCount = 18,
                    IsFeatured = true,
                    IsNew = false,
                    IsBestSeller = true,
                    CategoryId = menCategory?.Id ?? 1,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-2.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-3.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "AeroBoost Cushion High-Tops",
                    Description = "Superior shock absorption with responsive dual-density midsole foam and ankle support.",
                    SKU = "JUAN-BOOST-03",
                    Price = 145.00m,
                    DiscountPercent = 20,
                    StockCount = 12,
                    IsFeatured = true,
                    IsNew = true,
                    IsBestSeller = false,
                    CategoryId = sportsCategory?.Id ?? 1,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-3.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-4.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "Elegance Grace Women Heel Loafer",
                    Description = "Sleek contemporary women footwear with premium soft insoles and stylish buckles.",
                    SKU = "JUAN-WMN-04",
                    Price = 85.00m,
                    DiscountPercent = 10,
                    StockCount = 15,
                    IsFeatured = false,
                    IsNew = true,
                    IsBestSeller = true,
                    CategoryId = womenCategory?.Id ?? 2,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-4.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-5.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "Velocity Trail Outdoor Hiker",
                    Description = "Rugged all-terrain water-resistant grip sole footwear built for endurance and hiking trips.",
                    SKU = "JUAN-TRL-05",
                    Price = 160.00m,
                    DiscountPercent = 25,
                    StockCount = 8,
                    IsFeatured = true,
                    IsNew = false,
                    IsBestSeller = true,
                    CategoryId = sportsCategory?.Id ?? 1,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-5.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-6.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "Canvas Casual Daily Slip-On",
                    Description = "Easy slip-on lightweight canvas shoes with breathable cotton lining and vulcanized sole.",
                    SKU = "JUAN-CNV-06",
                    Price = 49.99m,
                    DiscountPercent = null,
                    StockCount = 30,
                    IsFeatured = false,
                    IsNew = false,
                    IsBestSeller = true,
                    CategoryId = casualCategory?.Id ?? 4,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-6.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-7.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "Phantom Black Performance Trainer",
                    Description = "Stealth edition all-black athletic gym and lifting shoe with reinforced heel counter.",
                    SKU = "JUAN-TRN-07",
                    Price = 115.00m,
                    DiscountPercent = 10,
                    StockCount = 20,
                    IsFeatured = true,
                    IsNew = true,
                    IsBestSeller = false,
                    CategoryId = sportsCategory?.Id ?? 1,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-7.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-8.jpg", IsMain = false, IsHover = true }
                    }
                },
                new Product
                {
                    Name = "Pastel Blossom Women Sneaker",
                    Description = "Vibrant pastel palette shoe with memory foam sockliner for cloud-like all-day walking.",
                    SKU = "JUAN-WMN-08",
                    Price = 95.00m,
                    DiscountPercent = null,
                    StockCount = 14,
                    IsFeatured = false,
                    IsNew = true,
                    IsBestSeller = true,
                    CategoryId = womenCategory?.Id ?? 2,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "product-8.jpg", IsMain = true, IsHover = false },
                        new ProductImage { ImageUrl = "product-9.jpg", IsMain = false, IsHover = true }
                    }
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // 7. Subscribers Seed
        if (!await context.Subscribers.IgnoreQueryFilters().AnyAsync())
        {
            var subscribers = new List<Subscriber>
            {
                new Subscriber { Email = "subscriber1@juan.com", IsActive = true },
                new Subscriber { Email = "subscriber2@juan.com", IsActive = true }
            };

            await context.Subscribers.AddRangeAsync(subscribers);
            await context.SaveChangesAsync();
        }
    }
}
