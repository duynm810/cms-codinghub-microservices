using Category.Grpc.Protos;
using Contracts.Commons.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Commons;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Domain.GrpcServices;
using Post.Domain.Interfaces;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Post.Infrastructure.GrpcServices;
using Post.Infrastructure.Persistence;
using Post.Infrastructure.Repositories;
using Post.Infrastructure.Services;
using PostInTag.Grpc.Protos;
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
            .AddScoped<ICategoryGrpcService, CategoryGrpcService>()
            .AddScoped<ITagGrpcService, TagGrpcService>()
            .AddScoped<IPostInTagGrpcService, PostInTagGrpcService>()
            .AddScoped<IPostEmailTemplateService, PostEmailTemplateService>()
            .AddScoped<ISerializeService, SerializeService>()
            .AddScoped<ICacheService, CacheService>();
    }

    private static void AddGrpcServices(this IServiceCollection services)
    {
        var grpcSettings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings)) ??
                           throw new ArgumentNullException(
                               $"{nameof(GrpcSettings)} is not configured properly");

        services.AddGrpcClient<CategoryProtoService.CategoryProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.CategoryUrl));

        services.AddGrpcClient<TagProtoService.TagProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.TagUrl));
        
        services.AddGrpcClient<PostInTagService.PostInTagServiceClient>(x =>
            x.Address = new Uri(grpcSettings.PostInTagUrl));
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }
}