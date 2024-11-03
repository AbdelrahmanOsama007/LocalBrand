# Use the official ASP.NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Create a non-root user (optional)
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser


# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["LocalBrand/LocalBrand.csproj", "LocalBrand/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Model/Model.csproj", "Model/"]

# Restore dependencies with detailed verbosity
RUN dotnet restore "LocalBrand/LocalBrand.csproj" --verbosity detailed

# Copy the rest of the files into the container
COPY . .

# Set the correct working directory for the build command
WORKDIR "/src/LocalBrand"

# Build the application
RUN dotnet build "LocalBrand.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "LocalBrand.csproj" -c Release -o /app/publish

# Final stage: Build a runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN dotnet dev-certs https --trust
ENTRYPOINT ["dotnet", "LocalBrand.dll"]
