FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApps/HealthCheck/WebApps.HealthCheck/WebApps.HealthCheck.csproj", "WebApps/HealthCheck/WebApps.HealthCheck/"]
RUN dotnet restore "WebApps/HealthCheck/WebApps.HealthCheck/WebApps.HealthCheck.csproj"
COPY . .
WORKDIR "/src/WebApps/HealthCheck/WebApps.HealthCheck"
RUN dotnet build "WebApps.HealthCheck.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApps.HealthCheck.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApps.HealthCheck.dll"]
