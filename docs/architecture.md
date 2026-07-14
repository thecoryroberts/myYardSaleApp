# Architecture

## Overview

myYardSale follows a clean architecture approach with clear separation between the UI, application services, domain entities, and infrastructure implementations.

## Layers

- Domain: contains core entities such as Listing and Category.
- Application: contains business services and abstractions for repositories.
- Infrastructure: contains concrete implementations and integration points.
- Web: hosts the MVC UI and presentation models.

## Design Goals

- Keep business rules in the domain and application layers.
- Use dependency injection for infrastructure services.
- Keep controllers thin and delegate to application services.
- Prepare the application for future EF Core, Identity, and API expansion.
