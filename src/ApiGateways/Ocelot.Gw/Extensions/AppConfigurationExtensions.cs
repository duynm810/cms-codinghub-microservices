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
            .AddEnvironmentVariables();

        // Calculate the correct path to the Swagger endpoints configuration based on the environment
        var swaggerEndpointsConfigPath = GetSwaggerEndpointsConfigPath(builder.Environment.EnvironmentName);

        // Validate if the Swagger endpoints configuration file exists
        ValidateSwaggerEndpointsConfigPath(swaggerEndpointsConfigPath);

        // Add Swagger endpoints configuration file
        builder.Configuration.AddJsonFile(swaggerEndpointsConfigPath, optional: false, reloadOnChange: true);

        // Add Ocelot configuration
        builder.Configuration.AddOcelot("Routes", builder.Environment);
    }

    private static string GetSwaggerEndpointsConfigPath(string environmentName)
    {
        return Path.Combine("Routes", environmentName, $"ocelot.SwaggerEndPoints.{environmentName}.json");
    }

    private static void ValidateSwaggerEndpointsConfigPath(string swaggerEndpointsConfigPath)
    {
        if (File.Exists(swaggerEndpointsConfigPath))
        {
            return;
        }
        
        Log.Fatal("Swagger endpoints configuration file '{FileName}' not found.", swaggerEndpointsConfigPath);
        throw new FileNotFoundException($"Configuration file '{swaggerEndpointsConfigPath}' not found.");
    }
}