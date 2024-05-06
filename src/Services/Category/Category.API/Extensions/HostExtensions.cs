using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Category.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetService<TContext>();

        try
        {
            if (context == null)
            {
                throw new Exception(
                    "Database context not found. Ensure the database context is registered and configured correctly.");
            }

            Log.Information("Migrating MySQL database");
            ExcuteMigrations(context);

            Log.Information("Migrated MySQL database");
            InvokeSeeder(seeder!, context, services);
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while migrating the MySQL database");
        }

        return host;
    }

    private static void ExcuteMigrations<TContext>(TContext context) where TContext : DbContext
    {
        context.Database.Migrate();
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services) where TContext : DbContext?
    {
        seeder(context, services);
    }
}