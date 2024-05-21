using Logging;
using Serilog;
using Shared.Constants;
using WebApps.HealthCheck.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {ApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Register app configuration settings
    builder.AddAppConfiguration();

    // Register application infrastructure services
    builder.Services.AddInfrastructureServices(configuration);

    builder.WebHost.UseWebRoot("wwwroot");

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, $"{ErrorMessageConsts.Common.UnhandledException}: {e.Message}");
    throw;
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}