using Logging;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Gw.Configs;
using Ocelot.Gw.Extensions;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

const string routes = "Routes";
var origins = configuration["AllowOrigins"];

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {ApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Load configuration from JSON files and environment variables
    builder.AddAppConfiguration();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", buider =>
        {
            if (origins != null)
                buider.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
    });
    
    builder.Services.AddSwaggerForOcelot(configuration); // Add this line to register SwaggerForOcelot services

    configuration.AddOcelotWithSwaggerSupport(options => { options.Folder = routes; });

    builder.Services.AddOcelot(configuration)
        .AddPolly()
        .AddCacheManager(x => x.WithDictionaryHandle());

    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
        .AddOcelot(routes, builder.Environment)
        .AddEnvironmentVariables();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Swagger for ocelot
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseRouting();

    app.UseCors("CorsPolicy");

    app.UseSwagger();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
        options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
    }).UseOcelot().Wait();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"{ErrorMessageConsts.Common.UnhandledException}: {ex.Message}");
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}