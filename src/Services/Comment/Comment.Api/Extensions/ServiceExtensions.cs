using Comment.Api.GrpcClients;
using Comment.Api.GrpcClients.Interfaces;
using Comment.Api.Repositories;
using Comment.Api.Repositories.Interfaces;
using Comment.Api.Services;
using Comment.Api.Services.Interfaces;
using Contracts.Commons.Interfaces;
using Identity.Grpc.Protos;
using Infrastructure.Commons;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Post.Grpc.Protos;
using Shared.Configurations;
using Shared.Settings;

namespace Comment.Api.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register database context
        services.ConfigureMongoDbClient();

        // Register Redis
        services.AddRedisConfiguration();

        // Register core services
        services.AddCoreInfrastructure();

        // Register repository and related services
        services.AddRepositoryAndDomainServices();

        // Register additional services
        services.AddAdditionalServices();

        // Register AutoMapper
        services.AddAutoMapperConfiguration();

        // Register Swagger services
        services.AddSwaggerConfiguration();

        // Register health checks
        services.AddHealthCheckServices();

        // Register gRPC services
        services.AddGrpcConfiguration();

        // Register authentication services
        services.AddAuthenticationServices();

        // Register authorization services
        services.AddAuthorizationServices();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var mongodbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(MongoDbSettings)} is not configured properly");

        services.AddSingleton(mongodbSettings);
        
        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>()
                            ?? throw new ArgumentNullException(
                                $"{nameof(CacheSettings)} is not configured properly");

        services.AddSingleton(cacheSettings);
    }

    private static string GetMongoConnectionString(this IServiceCollection services)
    {
        var mongodbSettings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings)) ??
                              throw new ArgumentNullException(
                                  $"{nameof(MongoDbSettings)} is not configured properly");
        
        // Build the MongoDB connection string using the settings and database name.
        var databaseName = mongodbSettings.DatabaseName;
        var mongodbConnectionString = mongodbSettings.ConnectionString + "/" + databaseName + "?authSource=admin";
        
        // Return the MongoDB connection string.
        return mongodbConnectionString;
    }
    
    private static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(GetMongoConnectionString(services)))
            .AddScoped(x => x.GetService<IMongoClient>()?.StartSession() 
                            ?? throw new ArgumentNullException(nameof(IMongoClient), "Mongo client instance is null"));
        
        // Register the GuidSerializer configuration for MongoDB (Đăng ký cấu hình GuidSerializer cho MongoDB)
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }
    
    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services
            .AddScoped<ICommentRepository, CommentRepository>()
            .AddScoped<ICommentService, CommentService>()
            .AddScoped<ISerializeService, SerializeService>()
            .AddScoped<ICacheService, CacheService>();
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void AddHealthCheckServices(this IServiceCollection services)
    {
        var mongodbSettings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings)) ??
                              throw new ArgumentNullException(
                                  $"{nameof(MongoDbSettings)} is not configured properly");
        
        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings)) ??
                            throw new ArgumentNullException(
                                $"{nameof(CacheSettings)} is not configured properly");
        
        var elasticsearchConfigurations = services.GetOptions<ElasticConfigurations>(nameof(ElasticConfigurations)) ??
                                          throw new ArgumentNullException(
                                              $"{nameof(ElasticConfigurations)} is not configured properly");

        services.AddHealthChecks()
            .AddMongoDb(mongodbSettings.ConnectionString,
                name: "MongoDb Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "mongo" })
            .AddRedis(cacheSettings.ConnectionString,
                name: "Redis Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "cache", "redis" })
            .AddElasticsearch(
                elasticsearchConfigurations.Uri,
                name: "Elasticsearch Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "search", "elasticsearch" });
    }

    private static void AddGrpcConfiguration(this IServiceCollection services)
    {
        var grpcSettings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings)) ??
                           throw new ArgumentNullException(
                               $"{nameof(GrpcSettings)} is not configured properly");
        
        services.AddGrpcClient<PostProtoService.PostProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.PostUrl));

        services.AddScoped<IPostGrpcClient, PostGrpcClient>();
        
        services.AddGrpcClient<UserProtoService.UserProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.IdentityUrl));

        services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();

    }
}