using Category.GRPC.Protos;
using Contracts.Domains.Repositories;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Domain.Interfaces;
using Post.Infrastructure.GrpcServices;
using Post.Infrastructure.Persistence;
using Post.Infrastructure.Repositories;
using Shared.Configurations;

namespace Post.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureSettings(configuration);

        // Configures and registers the database context with the service collection
        services.ConfigureDbContext(configuration);

        // Configures and registers to seed post data
        services.ConfigureSeedData();

        // Configures and registers core services
        services.ConfigureCoreServices();

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures and registers grpc services
        services.ConfigureGrpcServices(configuration);
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
        
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>() 
                           ?? throw new ArgumentNullException($"{nameof(GrpcSettings)} is not configured properly");
        
        services.AddSingleton(grpcSettings);
    }

    private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        {
            throw new ArgumentNullException(
                $"{nameof(DatabaseSettings)} is not configured properly");
        }

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

    private static void ConfigureSeedData(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeeder, PostSeedData>();
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
        services.AddScoped<IPostRepository, PostRepository>()
            .AddScoped<ICategoryGrpcService, CategoryGrpcService>();
    }
    
    private static void ConfigureGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>() 
                           ?? throw new ArgumentNullException($"{nameof(GrpcSettings)} is not configured properly");
        
        services.AddGrpcClient<CategoryProtoService.CategoryProtoServiceClient>(x =>
            x.Address = new Uri(grpcSettings.CategoryUrl));
        
        services.AddScoped<CategoryGrpcService>();
    }
}