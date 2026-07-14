using Microsoft.EntityFrameworkCore;
using myYardSale.Domain.Entities;
using myYardSale.Infrastructure.Persistence;

namespace myYardSale.Web.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyYardSaleDbContext>();
        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Categories.AnyAsync())
        {
            dbContext.Categories.AddRange(
                new Category { Name = "Furniture", Description = "Household furniture" },
                new Category { Name = "Electronics", Description = "Gadgets and electronics" },
                new Category { Name = "Tools", Description = "Tools and hardware" });

            await dbContext.SaveChangesAsync();
        }
    }
}
