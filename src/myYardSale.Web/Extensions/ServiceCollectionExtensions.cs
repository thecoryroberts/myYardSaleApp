using Microsoft.EntityFrameworkCore;
using myYardSale.Application.Abstractions;
using myYardSale.Application.Services;
using myYardSale.Infrastructure.Persistence;

namespace myYardSale.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=myYardSale.db";

        services.AddDbContext<MyYardSaleDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IListingRepository, SqliteListingRepository>();
        services.AddScoped<ListingService>();

        return services;
    }
}
