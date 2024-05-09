FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Category/Category.GRPC/Category.GRPC.csproj", "Services/Category/Category.GRPC/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/Category/Category.GRPC/Category.GRPC.csproj"
COPY . .
WORKDIR "/src/Services/Category/Category.GRPC"
RUN dotnet build "Category.GRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Category.GRPC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Category.GRPC.dll"]
