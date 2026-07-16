using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using myYardSale.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

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

// Health Checks - simple ping-style health check
builder.Services.AddHealthChecks();

// Production Security Headers
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
    });

    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 443;
    });
}

var app = builder.Build();

// Security Headers Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    
    // Security headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        await next();
    });
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

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
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
            ? new { status = "Healthy", timestamp = DateTime.UtcNow }
            : new { status = report.Status.ToString(), timestamp = DateTime.UtcNow };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run();
