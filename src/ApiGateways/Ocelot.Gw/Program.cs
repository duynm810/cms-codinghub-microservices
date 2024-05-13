using Logging;
using Ocelot.Gw.Extensions;
using Ocelot.Middleware;
using Serilog;

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
    
    builder.Services.AddInfrastructureServices(configuration);
    
    var app = builder.Build();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }
    
    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();
    
    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}