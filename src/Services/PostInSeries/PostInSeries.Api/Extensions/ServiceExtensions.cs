using Contracts.Commons.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Commons;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Post.Grpc.Protos;
using PostInSeries.Api.GrpcServices;
using PostInSeries.Api.GrpcServices.Interfaces;
using PostInSeries.Api.Repositories;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services;
using PostInSeries.Api.Services.Interfaces;
using Series.Grpc.Protos;
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
        // Register configuration settings
        services.ConfigureSettings(configuration);

        // Register Redis
        services.ConfigureRedis();

        // Register core services
        services.ConfigureCoreServices();

        // Register repository services
        services.ConfigureRepositoryServices();

        // Register additional services
        services.ConfigureOtherServices();

        // Register AutoMapper
        services.ConfigureAutoMapper();

        // Register Swagger services
        services.ConfigureSwaggerServices();

        // Register health checks
        services.ConfigureHealthChecks();
    
        // Register gRPC services
        services.ConfigureGrpcServices();
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
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
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = $"{SwaggerConsts.PostInSeriesApi} for Administrators",
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
            .AddScoped<IPostInSeriesService, PostInSeriesService>()
            .AddScoped<ISerializeService, SerializeService>();
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

    private static void ConfigureGrpcServices(this IServiceCollection services)
    {
        var grpcSettings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings)) ??
                           throw new ArgumentNullException(
                               $"{nameof(GrpcSettings)} is not configured properly");

        services.AddGrpcClient<PostProtoService.PostProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.PostUrl));
        
        services.AddScoped<IPostGrpcService, PostGrpcService>();

        services.AddGrpcClient<SeriesProtoService.SeriesProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.SeriesUrl));

        services.AddScoped<ISeriesGrpcService, SeriesGrpcService>();
    }
}