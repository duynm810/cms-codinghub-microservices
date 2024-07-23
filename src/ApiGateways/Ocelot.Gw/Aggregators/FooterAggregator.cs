using ILogger = Serilog.ILogger;

namespace Ocelot.Gw.Aggregators;

public class FooterAggregator(ILogger logger) : BaseAggregator(logger)
{
    protected override Dictionary<string, string> ResponseDictionary { get; } = new()
    {
        { "categories", "categories" },
        { "tags", "tags" }
    };
}