FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/PostInSeries/PostInSeries.Grpc/PostInSeries.Grpc.csproj", "Services/PostInSeries/PostInSeries.Grpc/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/PostInSeries/PostInSeries.Grpc/PostInSeries.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/PostInSeries/PostInSeries.Grpc"
RUN dotnet build "PostInSeries.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostInSeries.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostInSeries.Grpc.dll"]
