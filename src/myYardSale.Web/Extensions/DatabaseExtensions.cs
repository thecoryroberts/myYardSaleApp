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

        if (!await dbContext.Categories.AnyAsync())
        {
            dbContext.Categories.AddRange(
                new Category { Name = "Furniture", Description = "Household furniture" },
                new Category { Name = "Electronics", Description = "Gadgets and electronics" },
                new Category { Name = "Tools", Description = "Tools and hardware" });

            await dbContext.SaveChangesAsync();
        }

        const string adminRoleName = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(adminRoleName));
        }

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
    }
}
