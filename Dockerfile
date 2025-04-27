# Use the official .NET 8.0 SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the contents of your project into the Docker image
COPY . ./

# Build and publish the application
RUN dotnet publish ./Kpakam.csproj -c Release -o out/api

# Use the official .NET 8.0 ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from the build stage to the runtime image
COPY --from=build-env /app/out/api .

# Expose port 80 for the web application
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Kpakam.dll"]