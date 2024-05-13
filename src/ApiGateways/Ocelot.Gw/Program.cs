using Infrastructure.Middlewares;
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

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        $"{builder.Environment.ApplicationName} v1"));

    app.UseCors("CorsPolicy");

    app.UseMiddleware<ErrorWrappingMiddleware>();

    app.UseAuthentication();
    
    app.UseRouting();
    
    app.UseAuthorization();

    app.MapGet("/", context =>
    {
        context.Response.Redirect("swagger/index.html");
        return Task.CompletedTask;
    });

    await app.UseOcelot();

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}