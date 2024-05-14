FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateways/Ocelot.Gw/Ocelot.Gw.csproj", "ApiGateways/Ocelot.Gw/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "ApiGateways/Ocelot.Gw/Ocelot.Gw.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Ocelot.Gw"
RUN dotnet build "Ocelot.Gw.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ocelot.Gw.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["ApiGateways/Ocelot.Gw/Routes", "./Routes"]
ENTRYPOINT ["dotnet", "Ocelot.Gw.dll"]
