FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
COPY ["Services/Post/Post.GRPC/Post.GRPC.csproj", "Services/Post/Post.GRPC/"]
COPY ["Services/Post/Post.Application/Post.Application.csproj", "Services/Post/Post.Application/"]
COPY ["Services/Post/Post.Infrastructure/Post.Infrastructure.csproj", "Services/Post/Post.Infrastructure/"]
COPY ["Services/Post/Post.Domain/Post.Domain.csproj", "Services/Post/Post.Domain/"]

RUN dotnet restore "Services/Post/Post.GRPC/Post.GRPC.csproj"
COPY . .
WORKDIR "/src/Services/Post/Post.GRPC"
RUN dotnet build "Post.GRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.GRPC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.GRPC.dll"]
