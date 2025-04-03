# Use official .NET Playwright image
FROM mcr.microsoft.com/playwright/dotnet:v1.51.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet build "./root/HyperCrawlX/HyperCrawlX.csproj" -c Release -o /app/build


# Publish stage
FROM build AS publish
RUN dotnet publish "./root/HyperCrawlX/HyperCrawlX.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HyperCrawlX.dll"]