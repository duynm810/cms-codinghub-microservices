using Ocelot.DependencyInjection;
using Serilog;

namespace Ocelot.Gw.Extensions;

public static class AppConfigurationExtensions
{
    /// <summary>
    /// Extends the WebApplicationBuilder to add application configuration from JSON files and environment variables.
    /// This method simplifies setting up configuration by consolidating the addition of JSON files and environment variables into one extension method.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <returns>The same WebApplicationBuilder instance for chaining.</returns>
    public static void AddAppConfiguration(this WebApplicationBuilder builder)
    {
        // Add base path for configuration
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}