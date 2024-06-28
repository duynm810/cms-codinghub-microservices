FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/BackgroundJobs/Hangfire.Api/Hangfire.Api.csproj", "Services/BackgroundJobs/Hangfire.Api/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]

RUN dotnet restore "Services/BackgroundJobs/Hangfire.Api/Hangfire.Api.csproj"
COPY . .
WORKDIR "/src/Services/BackgroundJobs/Hangfire.Api"
RUN dotnet build "Hangfire.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hangfire.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hangfire.Api.dll"]
