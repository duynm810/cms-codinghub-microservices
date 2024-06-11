using Contracts.Commons.Interfaces;
using Infrastructure.Commons;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
        services.AddDatabaseContext();

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

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void AddHealthCheckServices(this IServiceCollection services)
    {
    }

    private static void AddGrpcConfiguration(this IServiceCollection services)
    {
    }
}