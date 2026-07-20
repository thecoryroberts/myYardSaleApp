# myYardSale

myYardSale is a production-oriented community marketplace application for organizing neighborhood yard sales, community events, and local listings. Built with ASP.NET Core MVC following Clean Architecture principles.

## Architecture

```
myYardSale.sln
├── src/
│   ├── myYardSale.Domain          # Business entities and rules
│   ├── myYardSale.Application     # Services and abstractions
│   ├── myYardSale.Infrastructure  # EF Core persistence implementations
│   └── myYardSale.Web            # ASP.NET Core MVC UI with Identity
└── tests/
    ├── myYardSale.UnitTests       # Service layer unit tests
    └── myYardSale.IntegrationTests # Repository integration tests
```

### Technology Stack

- **Runtime**: .NET 9.0
- **Framework**: ASP.NET Core MVC
- **ORM**: Entity Framework Core 9.0
- **Database**: SQLite (development), SQL Server (production)
- **Authentication**: ASP.NET Core Identity
- **Authorization**: Role-based (Admin, Seller) + resource ownership
- **Frontend**: Bootstrap 5, Bootstrap Icons, jQuery validation
- **Testing**: xUnit, Moq

## Features

- Browse listings with search, category filtering, and sorting
- Create, edit, and manage listings with image uploads
- Shopping cart and checkout system
- Order history and status tracking
- Admin panel for managing all listings
- Role-based access control (Admin, Seller)
- Responsive design for mobile and desktop
- Security headers (CSP, X-Frame-Options, etc.)
- Rate limiting and response compression
- Health check endpoint

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQLite (included) or SQL Server for production

### Quick Start

```bash
# Clone the repository
git clone https://github.com/thecoryroberts/myYardSaleApp.git myYardSale
cd myYardSale

# Build and restore
dotnet restore
dotnet build

# Run tests
dotnet test

# Run the application (SQLite by default)
cd src/myYardSale.Web
dotnet run
```

The application starts on `https://localhost:5001` with automatic database migrations.

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `Database:Provider` | Database provider (`sqlite` or `sqlserver`) | `sqlite` |
| `Database:ApplyMigrationsOnStartup` | Auto-migrate on startup | `true` |
| `ConnectionStrings:DefaultConnection` | Database connection string | `Data Source=myYardSale.db` |
| `Admin:Email` | Default admin email | Required via User Secrets |
| `Admin:Password` | Default admin password | Required via User Secrets |

### User Secrets (Development)

Configure admin credentials securely:

```bash
dotnet user-secrets init
dotnet user-secrets set "Admin:Email" "admin@myyardsale.com"
dotnet user-secrets set "Admin:Password" "Harpoon12!"
```

### Default Users (Development Seed Data)

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@myyardsale.com | Configured via User Secrets |
| Seller | seller1@myyardsale.com | Password123! |
| Buyer | buyer1@myyardsale.com | Password123! |
| Seller | seller2@myyardsale.com | Password123! |

> **Security Note**: Change all default passwords before deploying to production.

## Deployment

### Production Considerations

1. Set `Database:Provider` to `sqlserver` in `appsettings.Production.json`
2. Configure a secure SQL Server connection string
3. Set `Database:ApplyMigrationsOnStartup` to `false` (use manual migrations)
4. Configure `Admin:Email` and `Admin:Password` via environment variables
5. Enable HSTS in production configuration (already configured)
6. Use a reverse proxy (nginx, IIS, or Azure Front Door)

### Docker

```bash
docker build -t myyardsale .
docker run -p 8080:80 myyardsale
```

### Health Check

The application exposes a health endpoint at `/health`.

## API Endpoints

All endpoints are MVC views, not REST APIs. Key routes:

| Path | Method | Auth | Description |
|------|--------|------|-------------|
| `/` | GET | None | Browse listings |
| `/Home/Create` | GET/POST | Seller/Admin | Create listing |
| `/Home/Edit/{id}` | GET/POST | Seller/Admin | Edit listing |
| `/Home/Delete/{id}` | POST | Seller/Admin | Delete listing |
| `/Cart` | GET | Authenticated | View cart |
| `/Cart/Checkout` | POST | Authenticated | Place order |
| `/Orders` | GET | Authenticated | Order history |
| `/Admin` | GET | Admin | Admin dashboard |
| `/health` | GET | None | Health check |

## Security

- Content Security Policy (CSP) headers
- Anti-forgery tokens on all POST requests
- Rate limiting (100 requests/minute)
- HSTS (production only)
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- Referrer-Policy: strict-origin-when-cross-origin
- Permissions-Policy: restricted
- Resource ownership validation (IDOR protection)
- Role-based authorization

## Performance

- Response compression (Brotli/GZip)
- Response caching
- Database health monitoring
- Database indexes on all foreign keys and frequently queried columns
- IMemoryCache for category data
- AsNoTracking for read queries

## Project Structure

```
src/myYardSale.Web/
├── Controllers/          # MVC controllers
│   └── Admin/           # Admin area controllers
├── Extensions/          # Extension methods (DI, DB init)
├── Models/              # ViewModels
├── Services/            # UI-layer services (image upload)
├── ViewComponents/      # View components (cart count)
├── Views/               # Razor views
├── wwwroot/             # Static files
├── Program.cs           # Application entry point
└── appsettings*.json    # Configuration
```

## Development Notes

- Use `dotnet watch run` for hot reload during development
- Use `dotnet ef migrations add MigrationName` to create new EF Core migrations
- Use `dotnet ef database update` to apply migrations manually
- The admin panel requires an "Admin" role assignment
- Listing images are stored in `wwwroot/uploads/listings/{listingId}/`