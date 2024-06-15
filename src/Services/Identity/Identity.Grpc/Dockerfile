FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Identity/Identity.Grpc/Identity.Grpc.csproj", "Services/Identity/Identity.Grpc/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]

RUN dotnet restore "Services/Identity/Identity.Grpc/Identity.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Identity/Identity.Grpc"
RUN dotnet build "Identity.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Grpc.dll"]
