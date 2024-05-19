using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.Configurations;

namespace Identity.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register additional services
        services.AddAdditionalServices();

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
            });
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
    }
}