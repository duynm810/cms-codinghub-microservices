using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Post.API.Extensions;
using Post.Application;
using Post.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start {ApplicationName} up", builder.Environment.ApplicationName);

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    
    //Config JSON files and environment variables
    builder.AddAppConfiguration();
    
    // Extracts configuration settings from appsettings.json and registers them with the service collection
    builder.Services.ConfigureSettings(configuration);
    
    // Configure health checks
    builder.Services.ConfigureHealthChecks(configuration);

    // Config application services in Post.Application
    builder.Services.AddApplicationServices();

    // Config infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);
  
    // Add services to the container.
    builder.Services.AddControllers();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Category API");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });
    }

    //app.UseHttpsRedirection();

    app.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapDefaultControllerRoute();

    app.Run();
}
catch (Exception e)
{
    var type = e.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(e, $"Unhandled exception: {e.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}