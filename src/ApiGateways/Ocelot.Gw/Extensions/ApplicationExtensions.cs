using Infrastructure.Middlewares;
using Ocelot.Middleware;
using Shared.Constants;

namespace Ocelot.Gw.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with essential middleware components such as Swagger UI, routing, and endpoint mapping.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
    public static void ConfigurePipeline(this IApplicationBuilder app)
    {
       
    }
}