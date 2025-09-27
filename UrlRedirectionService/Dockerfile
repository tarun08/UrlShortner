# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY UrlRedirectionService.csproj ./

RUN dotnet restore

# Copy all and publish
COPY . ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "UrlRedirectionService.dll"]
