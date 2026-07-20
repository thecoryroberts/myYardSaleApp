using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using myYardSale.Web.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using myYardSale.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// User Secrets in Development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Services
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });
builder.Services.AddRazorPages();
builder.Services.AddApplicationServices(builder.Configuration);

// Authorization - only Admin and Seller roles can manage listings
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageListings", policy =>
        policy.RequireRole("Admin", "Seller"));
});

// Response Caching
builder.Services.AddResponseCaching();

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Health Checks - simple ping-style health check
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MyYardSaleDbContext>();

// Forwarded Headers for production behind reverse proxy
if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

// Production Security Headers
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 443;
    });
}

var app = builder.Build();

// Forwarded Headers
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}

// Security Headers Middleware (applied in all environments)
app.UseSecurityHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseResponseCompression();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

await app.InitializeDatabaseAsync(builder.Configuration);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = report.Status == HealthStatus.Healthy
            ? new { status = "Healthy", timestamp = DateTime.UtcNow }
            : new { status = report.Status.ToString(), timestamp = DateTime.UtcNow };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run();