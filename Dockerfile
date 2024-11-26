# Use the official ASP.NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-amd64 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Setting Environment Variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install EF Core tools globally

ENV PATH="$PATH:/root/.dotnet/tools"


RUN dotnet tool install --global dotnet-ef

# Ensure the dotnet tools are accessible for the current shell

# Copy project files
COPY ["LocalBrand/LocalBrand.csproj", "LocalBrand/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Model/Model.csproj", "Model/"]

# Restore dependencies with detailed verbosity
RUN dotnet restore "LocalBrand/LocalBrand.csproj" --verbosity detailed

# Copy the rest of the files into the container
COPY . .

WORKDIR "/src"


# Set the correct working directory for the build command


RUN dotnet ef migrations add SecondMigration --project Infrastructure --startup-project LocalBrand 


# Build the application


WORKDIR "/src/LocalBrand"


RUN dotnet build "LocalBrand.csproj" -c Release -o /app/build



# Publish the app
FROM build AS publish
RUN dotnet publish "LocalBrand.csproj" -c Release -o /app/publish

# Final stage: Build a runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# Entry point to run the application
ENTRYPOINT ["dotnet", "LocalBrand.dll"]
