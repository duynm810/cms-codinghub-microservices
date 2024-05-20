using Serilog;
using Shared.Constants;

namespace Identity.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Identity Swagger UI";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.IdentityApi} v1");
            c.DisplayRequestDuration();
        });
        
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