FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Tag/Tag.Grpc/Tag.Grpc.csproj", "Services/Tag/Tag.Grpc/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/Tag/Tag.Grpc/Tag.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Tag/Tag.Grpc"
RUN dotnet build "Tag.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tag.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tag.Grpc.dll"]
