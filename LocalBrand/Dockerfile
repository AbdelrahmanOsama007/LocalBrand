# Use the official ASP.NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Create a non-root user (optional)
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LocalBrand/LocalBrand.csproj", "./"]
RUN dotnet restore "LocalBrand.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LocalBrand.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "LocalBrand.csproj" -c Release -o /app/publish

# Final stage: Build a runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocalBrand.dll"]
