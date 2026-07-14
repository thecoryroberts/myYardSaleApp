# myYardSale

myYardSale is a production-oriented community marketplace application for organizing neighborhood yard sales, community events, and local listings.

## Current Milestone

This repository now includes:

- A clean-architecture solution structure with separate domain, application, infrastructure, web, and test projects
- A domain model for listings and categories
- A listing search service with unit tests
- A web UI for browsing and searching listings

## Architecture Overview

- Domain: business entities and rules
- Application: services and abstractions
- Infrastructure: persistence implementations and external integrations
- Web: ASP.NET Core MVC UI
- Tests: unit and integration test coverage

## Getting Started

```bash
dotnet build
```

## Next Steps

- Add EF Core persistence and SQL Server/SQLite support
- Introduce Identity and authentication
- Add organizations, events, households, and reservations
- Add Docker, CI/CD, and documentation assets
