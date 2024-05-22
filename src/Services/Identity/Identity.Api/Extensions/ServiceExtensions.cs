using Contracts.Domains.Repositories;
using Identity.Api.Services;
using Identity.Api.Services.Intefaces;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Services.Interfaces;
using Identity.Presentation;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Shared.Configurations;
using Shared.Constants;
using Shared.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Fix can't login same site
        services.ConfigureCookiePolicy();

        // Register core services
        services.AddCoreInfrastructure();

        // Register repository and related services
        services.AddRepositoryAndDomainServices();

        // Register AutoMapper
        services.AddAutoMapperConfiguration();

        // Register additional services
        services.AddAdditionalServices();

        // Register Swagger services
        services.AddSwaggerConfiguration(configuration);

        // Register identity
        services.AddIdentity();

        // Register identity server
        services.AddIdentityServer(configuration);

        // Register CORS services
        services.AddCorsConfiguration();

        // Register authentication
        services.AddAuthenticationConfiguration();

        // Register authorization
        services.AddAuthorizationServices();
        
        // Register health checks
        services.AddHealthCheckServices();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
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
            .AddScoped<IIdentityProfileService, IdentityProfileService>()
            .AddScoped<IIdentityReposityManager, IdentityRepositoryManager>()
            .AddScoped<IPermissionRepository, PermissionRepository>()
            .AddScoped<IPermissionService, PermissionService>();
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services
            .AddDbContext<IdentityContext>(options => { options.UseSqlServer(databaseSettings.ConnectionString); })
            .AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();
    }

    private static void AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        var issuerUri = configuration.GetSection("IdentityServer:IssuerUri").Value;
        services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
                options.EmitStaticAudienceClaim = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.IssuerUri = issuerUri;
            })
            .AddDeveloperSigningCredential() // not recommended for production - you need to store your key material somewhere source
            /*.AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddTestUsers(TestUsers.Users)*/
            .AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = c =>
                {
                    c.UseSqlServer(databaseSettings.ConnectionString,
                        builder => builder.MigrationsAssembly("Identity.Api"));
                };
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = c =>
                {
                    c.UseSqlServer(databaseSettings.ConnectionString,
                        builder => builder.MigrationsAssembly("Identity.Api"));
                };
            })
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>();
    }

    private static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }
    
    private static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{SwaggerConsts.IdentityApi} for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
            
            var identityServerBaseUrl = configuration.GetSection("IdentityServer:BaseUrl").Value;
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
                        AuthorizationUrl = new Uri($"{identityServerBaseUrl}/connect/authorize"),
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

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        // uncomment if you want to add a UI
        services.AddRazorPages();
        services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
            config.Filters.Add(new ProducesAttribute("application/json", "text/plain", "text/json"));
        }).AddApplicationPart(typeof(AssemblyReference).Assembly);
        
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void AddAuthenticationConfiguration(this IServiceCollection services)
    {
        services
            .AddAuthentication()
            .AddLocalApi(IdentityServerAuthenticationDefaults.AuthenticationScheme, option =>
            {
                option.ExpectedScope = "coding_hub_microservices_api.read";
            }); // Any token with this scope will be accepted. (Bất kỳ token nào có scope này sẽ được chấp nhận.)
    }
    
    private static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddSqlServer(databaseSettings.ConnectionString,
                name: "SqlServer Health",
                failureStatus: HealthStatus.Degraded);
    }
}