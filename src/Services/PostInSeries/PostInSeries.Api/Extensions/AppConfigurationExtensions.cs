namespace PostInSeries.Api.Extensions;

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
        // Add configuration from a JSON file named 'appsettings.json'. This file is mandatory (optional: false).
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
                reloadOnChange: true);

        // Add configuration from environment variables.
        // This includes variables set in the system and potentially, specific deployment settings.
        builder.Configuration.AddEnvironmentVariables();
    }
}