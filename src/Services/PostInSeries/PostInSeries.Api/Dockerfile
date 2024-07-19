FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/PostInSeries/PostInSeries.Api/PostInSeries.Api.csproj", "Services/PostInSeries/PostInSeries.Api/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/PostInSeries/PostInSeries.Api/PostInSeries.Api.csproj"
COPY . .
WORKDIR "/src/Services/PostInSeries/PostInSeries.Api"
RUN dotnet build "PostInSeries.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostInSeries.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostInSeries.Api.dll"]
