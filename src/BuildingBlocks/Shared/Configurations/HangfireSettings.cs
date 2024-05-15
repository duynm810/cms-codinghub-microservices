namespace Shared.Configurations;

public class HangfireSettings
{
    public string Route { get; set; } = default!;

    public string ServerName { get; set; } = default!;

    public DatabaseSettings Storage { get; set; } = default!;

    public Dashboard Dashboard { get; set; } = default!;
}

public class Dashboard
{
    public string AppPath { get; set; } = default!;

    public int StartsPollingInterval { get; set; } = default!;

    public string DashboardTitle { get; set; } = default!;
}