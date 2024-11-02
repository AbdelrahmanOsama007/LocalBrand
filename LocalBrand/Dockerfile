# Use the ASP.NET base image for the final application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["LocalBrand/LocalBrand.csproj", "LocalBrand/"]
RUN dotnet restore "LocalBrand/LocalBrand.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src/LocalBrand"
RUN dotnet build "LocalBrand.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "LocalBrand.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage to create the application image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocalBrand.dll"]
