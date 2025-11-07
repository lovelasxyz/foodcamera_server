# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/Cases.API/Cases.API.csproj", "src/Cases.API/"]
COPY ["src/Cases.Application/Cases.Application.csproj", "src/Cases.Application/"]
COPY ["src/Cases.Domain/Cases.Domain.csproj", "src/Cases.Domain/"]
COPY ["src/Cases.Infrastructure/Cases.Infrastructure.csproj", "src/Cases.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Cases.API/Cases.API.csproj"

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/src/Cases.API"
RUN dotnet build "Cases.API.csproj" -c Release -o /app/build
RUN dotnet publish "Cases.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

# Copy published files
COPY --from=build /app/publish .

# Set environment to production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Cases.API.dll"]
