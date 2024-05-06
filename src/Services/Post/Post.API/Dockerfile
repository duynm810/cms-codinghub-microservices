FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Post/Post.API/Post.API.csproj", "Services/Post/Post.API/"]
COPY ["Services/Post/Post.Application/Post.Application.csproj", "/Services/Post/Post.Application/"]
COPY ["Services/Post/Post.Domain/Post.Domain.csproj", "Services/Post/Post.Domain/"]
COPY ["Services/Post/Post.Infrastructure/Post.Infrastructure.csproj", "Services/Post/Post.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]

RUN dotnet restore "Services/Post/Post.API/Post.API.csproj"
COPY . .
WORKDIR "/src/Services/Post/Post.API"
RUN dotnet build "Post.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.API.dll"]
