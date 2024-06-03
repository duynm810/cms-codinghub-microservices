using WebApps.UI.Routes;

namespace WebApps.UI.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();
        
        // Register routes
        RouteMap.RegisterRoutes(app);
    }
}