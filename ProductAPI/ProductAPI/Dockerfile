# Stage 1: Build Environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ProductAPI/ProductAPI.csproj", "ProductAPI/"]
COPY ["ProductAPI.Tests/ProductAPI.Tests.csproj", "ProductAPI.Tests/"]
RUN dotnet restore "ProductAPI/ProductAPI.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/ProductAPI"
RUN dotnet build "ProductAPI.csproj" -c Release -o /app/build

# Stage 2: Test Environment (Optional - can be skipped in production)
FROM build AS test
WORKDIR /src
RUN dotnet test "ProductAPI.Tests/ProductAPI.Tests.csproj" \
    --configuration Release \
    --no-build \
    --verbosity normal \
    --collect:"XPlat Code Coverage"

# Stage 3: Publish
FROM build AS publish
WORKDIR "/src/ProductAPI"
RUN dotnet publish "ProductAPI.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    --no-build

# Stage 4: Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "ProductAPI.dll"]