using Serilog;

namespace Identity.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseSerilogRequestLogging();

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();
        
        app.UseCors("CorsPolicy");

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();
    }
}