using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myYardSale.Domain.Entities;
using myYardSale.Infrastructure.Persistence;

namespace myYardSale.Web.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
        {
            return;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyYardSaleDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync();

        // Seed Categories
        if (!await dbContext.Categories.AnyAsync())
        {
            dbContext.Categories.AddRange(
                new Category { Name = "Furniture", Description = "Household furniture, chairs, tables, sofas", IsActive = true },
                new Category { Name = "Electronics", Description = "Gadgets, phones, computers, TVs", IsActive = true },
                new Category { Name = "Tools", Description = "Power tools, hand tools, garden equipment", IsActive = true },
                new Category { Name = "Clothing", Description = "Men's, women's, kids' clothing and accessories", IsActive = true },
                new Category { Name = "Books", Description = "Fiction, non-fiction, textbooks, magazines", IsActive = true },
                new Category { Name = "Sports", Description = "Equipment, balls, bikes, outdoor gear", IsActive = true },
                new Category { Name = "Toys", Description = "Kids toys, games, puzzles", IsActive = true },
                new Category { Name = "Home & Garden", Description = "Kitchen, decor, plants, outdoor furniture", IsActive = true });

            await dbContext.SaveChangesAsync();
        }

        // Seed Roles
        const string adminRoleName = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(adminRoleName));
        }

        // Seed Admin User
        var adminEmail = configuration.GetValue<string>("Admin:Email") ?? "admin@myyardsale.com";
        var adminPassword = configuration.GetValue<string>("Admin:Password") ?? "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator",
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException("Unable to create the default admin user: " + string.Join(", ", createResult.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
        }

        // Seed Demo Users
        if (!await userManager.Users.AnyAsync(u => u.Email != adminEmail))
        {
            var demoUsers = new[]
            {
                new ApplicationUser { UserName = "seller1@myyardsale.com", Email = "seller1@myyardsale.com", FullName = "Jane Seller", IsActive = true },
                new ApplicationUser { UserName = "buyer1@myyardsale.com", Email = "buyer1@myyardsale.com", FullName = "Bob Buyer", IsActive = true },
                new ApplicationUser { UserName = "seller2@myyardsale.com", Email = "seller2@myyardsale.com", FullName = "Alice Vendor", IsActive = true }
            };

            foreach (var user in demoUsers)
            {
                await userManager.CreateAsync(user, "Password123!");
            }
        }

        // Seed Listings
        if (!await dbContext.Listings.AnyAsync())
        {
            var categories = await dbContext.Categories.ToListAsync();
            var users = await userManager.Users.ToListAsync();
            
            var demoListings = new[]
            {
                new Listing
                {
                    Title = "Vintage Wooden Coffee Table",
                    Description = "Beautiful mid-century modern coffee table in excellent condition. Solid wood with unique design. Pickup only.",
                    Price = 45.00m,
                    Status = ListingStatus.Active,
                    CategoryId = categories.First(c => c.Name == "Furniture").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
                },
                new Listing
                {
                    Title = "iPhone 14 Pro - Excellent Condition",
                    Description = "iPhone 14 Pro 128GB in excellent condition. No scratches, comes with original box and charger.",
                    Price = 750.00m,
                    Status = ListingStatus.Active,
                    CategoryId = categories.First(c => c.Name == "Electronics").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-3)
                },
                new Listing
                {
                    Title = "Cordless Drill Set - Barely Used",
                    Description = "18V cordless drill with two batteries and charger. Perfect for small home projects.",
                    Price = 35.00m,
                    Status = ListingStatus.Active,
                    CategoryId = categories.First(c => c.Name == "Tools").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-2)
                },
                new Listing
                {
                    Title = "Designer Handbag - Never Used",
                    Description = "Authentic designer handbag, never used, with dust bag and receipt.",
                    Price = 120.00m,
                    Status = ListingStatus.Active,
                    CategoryId = categories.First(c => c.Name == "Clothing").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
                },
                new Listing
                {
                    Title = "Harry Potter Book Collection - All 7 Books",
                    Description = "Complete Harry Potter hardcover collection. Great condition, perfect for collectors or fans.",
                    Price = 65.00m,
                    Status = ListingStatus.Active,
                    CategoryId = categories.First(c => c.Name == "Books").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-4)
                },
                new Listing
                {
                    Title = "Mountain Bike - Size Large",
                    Description = "21-speed mountain bike, recently tuned up. Great for trails or commuting.",
                    Price = 150.00m,
                    Status = ListingStatus.Draft,
                    CategoryId = categories.First(c => c.Name == "Sports").Id,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
                }
            };

            dbContext.Listings.AddRange(demoListings);
            await dbContext.SaveChangesAsync();
        }
    }
}
