namespace Shared.Settings;

public class DatabaseSettings
{
    public string DbProvider { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;
}