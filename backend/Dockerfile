# Multi-stage build - Backend API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY EcomApi.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage - minimal image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /sbin/nologin -D appuser

# Copy published app
COPY --from=build --chown=appuser:appuser /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD dotnet EcomApi.dll --health-check || exit 1

# Run as non-root
USER appuser

# Expose port
EXPOSE 5000

# Labels for metadata
LABEL org.opencontainers.image.title="E-Commerce API"
LABEL org.opencontainers.image.description="Secure .NET 8.0 API"
LABEL org.opencontainers.image.version="1.0.0"

ENTRYPOINT ["dotnet", "EcomApi.dll"]
