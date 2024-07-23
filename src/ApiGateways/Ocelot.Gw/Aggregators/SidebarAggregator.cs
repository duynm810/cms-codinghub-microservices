using ILogger = Serilog.ILogger;

namespace Ocelot.Gw.Aggregators;

public class SidebarAggregator(ILogger logger) : BaseAggregator(logger)
{
    protected override Dictionary<string, string> ResponseDictionary { get; } = new()
    {
        { "most-commented-posts", "most-commented-posts" },
        { "latest-comments", "latest-comments" }
    };
}