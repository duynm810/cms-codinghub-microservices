using Category.Api.GrpcServices;
using Category.Api.GrpcServices.Interfaces;
using Category.Api.Persistence;
using Category.Api.Repositories;
using Category.Api.Repositories.Interfaces;
using Category.Api.Services;
using Category.Api.Services.Interfaces;
using Contracts.Domains.Repositories;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Post.Grpc.Protos;
using Shared.Configurations;
using Shared.Constants;
using Shared.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Category.Api.Extensions;

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
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);

        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>()
                           ?? throw new ArgumentNullException($"{nameof(GrpcSettings)} is not configured properly");

        services.AddSingleton(grpcSettings);
        
        var apiConfigurations = configuration.GetSection(nameof(ApiConfigurations)).Get<ApiConfigurations>()
                           ?? throw new ArgumentNullException($"{nameof(ApiConfigurations)} is not configured properly");

        services.AddSingleton(apiConfigurations);
    }

    private static void AddDatabaseContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
        services.AddDbContext<CategoryContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly(SwaggerConsts.CategoryApi);
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        var apiConfigurations = services.GetOptions<ApiConfigurations>(nameof(ApiConfigurations)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(ApiConfigurations)} is not configured properly");
        
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc(apiConfigurations.ApiVersion, new OpenApiInfo
            {
                Version = apiConfigurations.ApiVersion,
                Title = $"{SwaggerConsts.CategoryApi} for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
            c.AddSecurityDefinition(IdentityServerAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                // Determine the security scheme type as OAuth2
                // Xác định loại scheme bảo mật là OAuth2
                Type = SecuritySchemeType.OAuth2, 
                Flows = new OpenApiOAuthFlows //  Supported OAuth2 flow definitions (Định nghĩa flow OAuth2 được hỗ trợ)
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        // The URL of the authorization endpoint where the user will be redirected for authentication.
                        // URL của endpoint ủy quyền, nơi người dùng sẽ được chuyển hướng đến để xác thực.
                        AuthorizationUrl = new Uri($"{apiConfigurations.IdentityServerBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "coding_hub_microservices_api.read", "Coding Hub Microservices API Read Scope" },
                            { "coding_hub_microservices_api.write", "Coding Hub Microservices API Write Scope" }
                        }
                    }
                },
                Description = "JWT Authorization header using the Bearer scheme. Example: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    // Determine security requirements for the API
                    // Xác định yêu cầu bảo mật cho API
                    new OpenApiSecurityScheme
                    {
                        // Reference to the security definition "Bearer".
                        // Tham chiếu đến định nghĩa bảo mật "Bearer".
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = IdentityServerAuthenticationDefaults.AuthenticationScheme
                        }
                    },
                    new List<string> //  List of scopes to which this security requirement applies (Danh sách các phạm vi (scopes) mà yêu cầu bảo mật này áp dụng)
                    {
                        "coding_hub_microservices_api.read",
                        "coding_hub_microservices_api.write"
                    }
                }
            });
        });
    }

    private static void AddCoreInfrastructure(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
            .AddScoped(typeof(IRepositoryCommandBase<,,>), typeof(RepositoryCommandBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICategoryService, CategoryService>();
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

        services.AddHealthChecks().AddMySql(connectionString: databaseSettings.ConnectionString,
            name: "MySQL Health",
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
    }
}