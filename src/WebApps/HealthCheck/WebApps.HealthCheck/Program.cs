using Logging;
using Serilog;
using WebApps.HealthCheck.Extensions;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Initialize console logging for application startup
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
    Log.Information("Starting up {ApplicationName}", builder.Environment.ApplicationName);

    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.AddAppConfiguration();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Registers health check UI
    builder.Services.AddHealthChecksUI().AddInMemoryStorage();

    builder.WebHost.UseWebRoot("wwwroot");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    // Add health check UI middleware
    app.MapHealthChecksUI();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception e)
{
    Log.Error(e.Message);
    throw;
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}