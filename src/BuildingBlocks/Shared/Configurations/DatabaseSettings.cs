namespace Shared.Configurations;

public class DatabaseSettings
{
    public required string DbProvider { get; set; }

    public required string ConnectionString { get; set; }

    public required string DatabaseName { get; set; }
}