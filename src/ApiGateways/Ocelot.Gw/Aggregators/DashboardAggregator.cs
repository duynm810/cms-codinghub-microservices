using ILogger = Serilog.ILogger;

namespace Ocelot.Gw.Aggregators;

public class DashboardAggregator(ILogger logger) : BaseAggregator(logger)
{
    protected override Dictionary<string, string> ResponseDictionary { get; } = new()
    {
        { "featured-posts", "featured-posts" },
        { "most-liked-posts", "most-liked-posts" },
        { "pinned-posts", "pinned-posts" },
        { "suggest-tags", "suggest-tags" }
    };
}