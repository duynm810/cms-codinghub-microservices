using Category.API.Extensions;
using Category.API.Persistence;
using Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start {ApplicationName} up", builder.Environment.ApplicationName);

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    
    // Config JSON files and environment variables
    builder.AddAppConfiguration();

    // Register services
    builder.Services.AddInfrastructureServices(configuration);

    var app = builder.Build();

    // Config pipeline
    app.ConfigurePipeline();

    await app.MigrateDatabase<CategoryContext>((context, _) => { CategorySeedData.CategorySeedAsync(context, Log.Logger).Wait(); })
        .RunAsync();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}