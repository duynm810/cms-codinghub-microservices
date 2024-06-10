FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/PostInTag/PostInTag.Grpc/PostInTag.Grpc.csproj", "Services/PostInTag/PostInTag.Grpc/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/PostInTag/PostInTag.Grpc/PostInTag.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/PostInTag/PostInTag.Grpc"
RUN dotnet build "PostInTag.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostInTag.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostInTag.Grpc.dll"]
