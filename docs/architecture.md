# Architecture

## Overview

myYardSale follows a clean architecture approach with clear separation between the UI, application services, domain entities, and infrastructure implementations.

## Layers

- **Domain**: Contains core entities such as Listing, Category, CartItem, Order, and related enums.
- **Application**: Contains business services (ListingService) and abstractions (IListingRepository).
- **Infrastructure**: Contains concrete implementations (SqliteListingRepository), Entity Framework DbContext, and migrations.
- **Web**: Hosts the MVC UI with controllers, views, view models, and custom services.

## Design Goals

- Keep business rules in the domain and application layers.
- Use dependency injection for infrastructure services.
- Keep controllers thin and delegate to application services.
- Prepare the application for future EF Core, Identity, and API expansion.

## Production Readiness Features

### Security
- HTTPS enforcement with HSTS
- Security headers (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy, Permissions-Policy)
- Anti-forgery protection on all forms
- Secure cookie policies configurable via appsettings

### Performance
- Response caching middleware
- Lazy loading on images
- Efficient database queries with async patterns
- CSS minification and bundling support

### Monitoring & Operations
- Configuration-based environment settings
- Structured logging with configurable levels
- Database migrations on startup (configurable)
- Health check endpoint for container orchestration

### Containerization
- Multi-stage Dockerfile for optimized builds
- Non-root user support
- Health check integration

## Deployment

### Environment Variables
| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Production/Development |
| `ConnectionStrings__DefaultConnection` | Database connection string |
| `Database__Provider` | sqlserver/sqlite |
| `Database__ApplyMigrationsOnStartup` | true/false |

### Docker
```bash
docker build -t myyardsale:latest .
docker run -d -p 80:80 -e ASPNETCORE_ENVIRONMENT=Production myyardsale:latest