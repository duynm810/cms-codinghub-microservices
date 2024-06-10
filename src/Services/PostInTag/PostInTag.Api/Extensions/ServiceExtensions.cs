using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Commons;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Post.Grpc.Protos;
using PostInTag.Api.GrpcServices;
using PostInTag.Api.GrpcServices.Interfaces;
using PostInTag.Api.Persistence;
using PostInTag.Api.Repositories;
using PostInTag.Api.Repositories.Interfaces;
using PostInTag.Api.Services;
using PostInTag.Api.Services.Interfaces;
using Shared.Settings;
using Tag.Grpc.Protos;

namespace PostInTag.Api.Extensions;

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

        // Register AutoMapper
        services.AddAutoMapperConfiguration();

        // Register Swagger services
        services.AddSwaggerConfiguration();

        // Register core services
        services.AddCoreInfrastructure();

        // Register repository services
        services.AddRepositoryAndDomainServices();

        // Register additional services
        services.AddAdditionalServices();

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
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);

        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>()
                            ?? throw new ArgumentNullException(
                                $"{nameof(CacheSettings)} is not configured properly");

        services.AddSingleton(cacheSettings);
    }

    private static void AddDatabaseContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddDbContextPool<PostInTagContext>(opts =>
        {
            opts.UseNpgsql(databaseSettings.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.UseNodaTime();
                optionsBuilder.MigrationsAssembly(typeof(PostInTagContext).Assembly.FullName);
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior
                    .SplitQuery); // If query have multiple include entities, using split query separate SQL query
            });
            opts.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services
            .AddScoped<IPostInTagRepository, PostInTagRepository>()
            .AddScoped<IPostInTagService, PostInTagService>()
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
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded);
    }

    private static void AddGrpcConfiguration(this IServiceCollection services)
    {
        var grpcSettings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings)) ??
                           throw new ArgumentNullException(
                               $"{nameof(GrpcSettings)} is not configured properly");
        
        services.AddGrpcClient<PostProtoService.PostProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.PostUrl));

        services.AddScoped<IPostGrpcService, PostGrpcService>();

        services.AddGrpcClient<TagProtoService.TagProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.TagUrl));

        services.AddScoped<ITagGrpcService, TagGrpcService>();

        services.AddGrpcClient<CategoryProtoService.CategoryProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.CategoryUrl));

        services.AddScoped<ICategoryGrpcService, CategoryGrpcService>();
    }
}