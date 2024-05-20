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
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Configurations;
using Shared.Constants;
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
        services.AddSwaggerConfiguration();

        // Register identity
        services.AddIdentity();

        // Register identity server
        services.AddIdentityServer();

        // Register CORS services
        services.AddCorsConfiguration();
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

    private static void AddIdentityServer(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
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
                        builder => builder.MigrationsAssembly("Identity.Infrastructure"));
                };
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = c =>
                {
                    c.UseSqlServer(databaseSettings.ConnectionString,
                        builder => builder.MigrationsAssembly("Identity.Infrastructure"));
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

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = $"{SwaggerConsts.IdentityApi} for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
        });
    }
}