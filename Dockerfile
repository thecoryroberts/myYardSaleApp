# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/myYardSale.Web/myYardSale.Web.csproj", "src/myYardSale.Web/"]
COPY ["src/myYardSale.Application/myYardSale.Application.csproj", "src/myYardSale.Application/"]
COPY ["src/myYardSale.Domain/myYardSale.Domain.csproj", "src/myYardSale.Domain/"]
COPY ["src/myYardSale.Infrastructure/myYardSale.Infrastructure.csproj", "src/myYardSale.Infrastructure/"]
COPY ["src/myYardSale.Shared/myYardSale.Shared.csproj", "src/myYardSale.Shared/"]

RUN dotnet restore "src/myYardSale.Web/myYardSale.Web.csproj"

# Copy source and build
COPY . .
WORKDIR "/src/src/myYardSale.Web"
RUN dotnet publish "myYardSale.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Copy published app
COPY --from=build /app/publish ./

# Expose ports
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "myYardSale.Web.dll"]