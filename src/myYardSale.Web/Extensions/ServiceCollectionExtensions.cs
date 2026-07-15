using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myYardSale.Application.Abstractions;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;
using myYardSale.Infrastructure.Persistence;
using myYardSale.Web.Services;

namespace myYardSale.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("Database:Provider")?.ToLowerInvariant() ?? "sqlite";
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=myYardSale.db";

        services.AddDbContext<MyYardSaleDbContext>(options =>
        {
            if (provider == "sqlserver")
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<MyYardSaleDbContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();

        services.AddScoped<IListingRepository, SqliteListingRepository>();
        services.AddScoped<ListingService>();
        services.AddScoped<ListingImageService>();

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        return services;
    }
}
