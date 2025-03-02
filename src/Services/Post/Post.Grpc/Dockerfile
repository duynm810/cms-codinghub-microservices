﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
COPY ["Services/Post/Post.Grpc/Post.Grpc.csproj", "Services/Post/Post.Grpc/"]
COPY ["Services/Post/Post.Infrastructure/Post.Infrastructure.csproj", "Services/Post/Post.Infrastructure/"]

RUN dotnet restore "Services/Post/Post.Grpc/Post.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Post/Post.Grpc"
RUN dotnet build "Post.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Grpc.dll"]
