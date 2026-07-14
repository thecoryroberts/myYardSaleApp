# myYardSale

myYardSale is a production-oriented community marketplace application for organizing neighborhood yard sales, community events, and local listings.

## Current Milestone

This repository now includes:

- A clean-architecture solution structure with separate domain, application, infrastructure, web, and test projects
- A domain model for listings, categories, organizations, events, households, and users
- A listing search service with unit and integration tests
- EF Core persistence with SQLite and SQL Server support
- ASP.NET Core Identity with authentication and role-based authorization (Admin role)
- A web UI for browsing, searching, creating, editing, and deleting listings
- An admin panel for managing all listings
- Database seeding for categories, roles, and default admin user

## Architecture Overview

- **Domain**: business entities and rules (Listing, Category, Organization, Event, Household, ApplicationUser)
- **Application**: services and abstractions (ListingService, IListingRepository)
- **Infrastructure**: persistence implementations (EF Core DbContext, SqliteListingRepository)
- **Web**: ASP.NET Core MVC UI with Identity UI
- **Tests**: unit and integration test coverage

## Getting Started

```bash
dotnet build
dotnet test
```

The application launches with SQLite by default. To enable automatic migrations on startup, set `Database:ApplyMigrationsOnStartup` to `true` in `appsettings.json`.

### Default Admin Credentials

- **Email**: admin@myyardsale.com
- **Password**: Admin123!

## Next Steps

- Add reservation system for listing bookings
- Add Docker, CI/CD, and documentation assets
- Add image upload support for listings
- Add real-time notifications
- Add API endpoints for mobile clients
