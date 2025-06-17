# Use the official .NET 6 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET 6 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out ./

# Expose the port your application runs on
EXPOSE 5000
EXPOSE 5001

# Set the entry point for the container
ENTRYPOINT ["dotnet", "WebAPI6.dll"]
