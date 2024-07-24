using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Settings;
using ILogger = Serilog.ILogger;

namespace Identity.Api.Seeds;

public static class IdentitySeed
{
    public static async Task<IHost> MigrateDatabaseAsync(this IHost host, IConfiguration configuration, ILogger logger)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");
        
        using var scope = host.Services.CreateScope();

        try
        {
            logger.Information("Starting database migration.");

            await using var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.SetConnectionString(databaseSettings.ConnectionString);
            await context.Database.MigrateAsync();

            await using var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            persistedGrantDbContext.Database.SetConnectionString(databaseSettings.ConnectionString);
            await persistedGrantDbContext.Database.MigrateAsync();

            await using var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            identityContext.Database.SetConnectionString(databaseSettings.ConnectionString);
            await identityContext.Database.MigrateAsync();

            logger.Information("Database migration completed.");

            logger.Information("Seeding data.");

            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }

                await context.SaveChangesAsync();
                logger.Information("Clients seeded.");
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                await context.SaveChangesAsync();
                logger.Information("IdentityResources seeded.");
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var apiScope in Config.ApiScopes)
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                }

                await context.SaveChangesAsync();
                logger.Information("ApiScopes seeded.");
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                await context.SaveChangesAsync();
                logger.Information("ApiResources seeded.");
            }

            await identityContext.SaveChangesAsync();
            logger.Information("Data seeding completed.");
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred during database migration or data seeding.");
            throw;
        }

        return host;
    }
}