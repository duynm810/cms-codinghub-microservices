using Logging;
using Serilog;
using WebApps.UI.Extensions;

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

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}