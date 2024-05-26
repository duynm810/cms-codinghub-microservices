namespace Shared.Settings;

public class DatabaseSettings
{
    public string? DbProvider { get; set; }

    public string? ConnectionString { get; set; }

    public string? DatabaseName { get; set; }
}