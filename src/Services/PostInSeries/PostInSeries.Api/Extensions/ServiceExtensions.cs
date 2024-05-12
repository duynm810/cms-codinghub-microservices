using Contracts.Domains.Repositories;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PostInSeries.Api.Repositories;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services;
using PostInSeries.Api.Services.Interfaces;
using Shared.Configurations;
using Shared.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PostInSeries.Api.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureSettings(configuration);

        // Configures and registers the redis
        services.ConfigureRedis();

        // Configures and registers core services
        services.ConfigureCoreServices();

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures and registers essential services
        services.ConfigureOtherServices();

        // Configures and registers AutoMapper
        services.ConfigureAutoMapper();

        // Configures swagger services
        services.ConfigureSwaggerServices();

        // Configure health checks
        services.ConfigureHealthChecks();
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);

        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>()
                            ?? throw new ArgumentNullException(
                                $"{nameof(CacheSettings)} is not configured properly");

        services.AddSingleton(cacheSettings);
    }

    private static void ConfigureRedis(this IServiceCollection services)
    {
        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings)) ??
                            throw new ArgumentNullException($"{nameof(DatabaseSettings)} is not configured properly");

        //Redis Configuration
        services.AddStackExchangeRedisCache(options => { options.Configuration = cacheSettings.ConnectionString; });
    }

    private static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc(SystemConsts.PostInSeriesApi, new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = "Post In Series Api for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
        });
    }

    private static void ConfigureCoreServices(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
            .AddScoped(typeof(IRepositoryCommandBase<,,>), typeof(RepositoryCommandBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }

    private static void ConfigureRepositoryServices(this IServiceCollection services)
    {
        services
            .AddScoped<IPostInSeriesRepository, PostInSeriesRepository>()
            .AddScoped<IPostInSeriesService, PostInSeriesService>();
    }

    private static void ConfigureOtherServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings)) ??
                            throw new ArgumentNullException($"{nameof(DatabaseSettings)} is not configured properly");

        services.AddHealthChecks()
            .AddRedis(cacheSettings.ConnectionString,
                "Redis Health",
                HealthStatus.Degraded);
    }
}