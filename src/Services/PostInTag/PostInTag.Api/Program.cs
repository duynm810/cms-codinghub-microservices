using Infrastructure.Extensions;
using Logging;
using PostInTag.Api.Extensions;
using PostInTag.Api.Persistence;
using Serilog;
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
    builder.Services.AddInfrastructureServices(configuration);

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();

    // Seed database with initial data and start the application
    app.MigrateDatabase();
    
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