using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Contracts.Domains.Repositories;
using Identity.Grpc.Protos;
using Infrastructure.Commons;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Domain.GrpcClients;
using Post.Domain.Interfaces;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Post.Infrastructure.GrpcClients;
using Post.Infrastructure.Persistence;
using Post.Infrastructure.Repositories;
using Post.Infrastructure.Services;
using PostInTag.Grpc.Protos;
using Series.Grpc.Protos;
using Shared.Settings;
using Tag.Grpc.Protos;

namespace Post.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register database context
        services.AddDatabaseContext();

        // Register Redis
        services.AddRedisConfiguration();

        // Register data seeding for posts
        services.AddSeedDataServices();

        // Register core services
        services.AddCoreInfrastructure();

        // Register repository services
        services.AddRepositoryAndDomainServices();

        // Register gRPC services
        services.AddGrpcServices();

        // Register AutoMapper
        services.AddAutoMapperConfiguration();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
        
        // Using IOptions for EventBusSettings (Sử dụng IOptions cho EventBusSettings)
        services.Configure<EventBusSettings>(configuration.GetSection(nameof(EventBusSettings)));
    }

    private static void AddDatabaseContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddDbContextPool<PostContext>(opts =>
        {
            opts.UseNpgsql(databaseSettings.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.UseNodaTime();
                optionsBuilder.MigrationsAssembly(typeof(PostContext).Assembly.FullName);
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior
                    .SplitQuery); // If query have multiple include entities, using split query separate SQL query
            });
            opts.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddSeedDataServices(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeeder, PostSeedData>();
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostRepository>()
            .AddScoped<IPostActivityLogRepository, PostActivityLogRepository>()
            .AddScoped<ICategoryGrpcClient, CategoryGrpcClient>()
            .AddScoped<ISeriesGrpcClient, SeriesGrpcClient>()
            .AddScoped<ITagGrpcClient, TagGrpcClient>()
            .AddScoped<IPostInTagGrpcClient, PostInTagGrpcClient>()
            .AddScoped<IIdentityGrpcClient, IdentityGrpcClient>()
            .AddScoped<IPostEmailTemplateService, PostEmailTemplateService>()
            .AddScoped<ISerializeService, SerializeService>()
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<IPostService, PostService>()
            .AddScoped<IPostEventService, PostEventService>();
    }

    private static void AddGrpcServices(this IServiceCollection services)
    {
        var grpcSettings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings)) ??
                           throw new ArgumentNullException(
                               $"{nameof(GrpcSettings)} is not configured properly");

        services.AddGrpcClient<CategoryProtoService.CategoryProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.CategoryUrl));
        
        services.AddGrpcClient<SeriesProtoService.SeriesProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.SeriesUrl));
        
        services.AddGrpcClient<TagProtoService.TagProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.TagUrl));
        
        services.AddGrpcClient<PostInTagService.PostInTagServiceClient>(x =>
            x.Address = new Uri(grpcSettings.PostInTagUrl));
        
        services.AddGrpcClient<UserProtoService.UserProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.IdentityUrl));
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }
}