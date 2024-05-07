using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Post.API.Extensions;
using Post.Application;
using Post.Domain.Interfaces;
using Post.Infrastructure;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

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
    //builder.Services.ConfigureHealthChecks(configuration);

    // Config application services in Post.Application
    builder.Services.AddApplicationServices();

    // Config infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);
  
    // Add services to the container.
    builder.Services.AddControllers();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
        c.SwaggerDoc("PostAPI", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Post API for Administrators",
            Description =
                "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
        });
    });
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Post Swagger UI";
            c.SwaggerEndpoint("PostAPI/swagger.json", $"{builder.Environment.ApplicationName} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });
    }
    
    // Initialise and seed database
    /*using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.InitialiseAsync();
    await seeder.SeedAsync();*/

    //app.UseHttpsRedirection();
    app.UseRouting();

    /*app.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapDefaultControllerRoute();*/

    app.Run();
}
catch (Exception e)
{
    var type = e.GetType().Name;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal)) throw;

    Log.Fatal(e, $"Unhandled exception: {e.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}