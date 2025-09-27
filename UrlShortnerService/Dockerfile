# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY UrlShortnerService.csproj ./
RUN dotnet restore

# Copy all and publish
COPY . ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output
COPY --from=build /app ./

# Force ASP.NET Core to listen on port 7100
ENV ASPNETCORE_URLS=http://+:7000
EXPOSE 7100

ENTRYPOINT ["dotnet", "UrlShortnerService.dll"]
