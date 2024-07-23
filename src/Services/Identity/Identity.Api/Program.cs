using Identity.Api.Extensions;
using Identity.Api.Seeds;
using Infrastructure.Extensions;
using Logging;
using Serilog;
using Serilog.Core;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {ApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Load configuration from JSON files and environment variables
    builder.AddAppConfiguration();

    // Register application infrastructure services
    builder.Services.AddInfrastructureService(configuration);

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();
    
    try
    {
        // Ensure the database is migrated and seeded before running the application
        await app.MigrateDatabaseAsync(builder.Configuration, Log.Logger);
         
        // Seed user sample data
        UserSeedData.EnsureSeedData(configuration);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during migration or seeding");
        throw;
    }
   
    // Run the application
    await app.RunAsync();
}
catch (Exception e)
{
    var type = e.GetType().Name;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal)) throw;

    Log.Fatal(e, $"{ErrorMessagesConsts.Common.UnhandledException}: {e.Message}");
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}