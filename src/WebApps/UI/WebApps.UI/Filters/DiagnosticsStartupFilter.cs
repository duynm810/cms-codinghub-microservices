namespace WebApps.UI.Filters;

public class DiagnosticsStartupFilter(IHostEnvironment hostEnvironment) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            if (!hostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            next(app);
        };
    }
}
