using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ocelot.Configuration;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using ILogger = Serilog.ILogger;

namespace Ocelot.Gw.Aggregators;

public class DashboardAggregator(ILogger logger) : IDefinedAggregator
{
    private static readonly Dictionary<string, string> ResponseDictionary = new()
    {
        { "featured-posts", "featured-posts" },
        { "most-liked-posts", "most-liked-posts" },
        { "pinned-posts", "pinned-posts" },
        { "suggest-tags", "suggest-tags" },
        { "latest-comments", "latest-comments" }
    };

    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        var aggregatedResponse = new JObject();

        var tasks = responses.Select(response => ProcessResponse(response, aggregatedResponse)).ToList();

        await Task.WhenAll(tasks);

        var firstResponse = responses.FirstOrDefault();

        return firstResponse?.Items.TryGetValue("DownstreamResponse", out var firstDownstreamResponseObject) == true &&
               firstDownstreamResponseObject is DownstreamResponse firstDownstreamResponse
            ? CreateAggregatedResponse(aggregatedResponse, firstDownstreamResponse)
            : CreateDefaultDownstreamResponse();
    }

    private async Task ProcessResponse(HttpContext response, JObject aggregatedResponse)
    {
        if (response.Items.TryGetValue("DownstreamRoute", out var downstreamRouteObject) &&
            response.Items.TryGetValue("DownstreamResponse", out var downstreamResponseObject) &&
            downstreamRouteObject is DownstreamRoute downstreamRoute &&
            downstreamResponseObject is DownstreamResponse downstreamResponse)
        {
            string content;
            try
            {
                content = await downstreamResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error reading content from DownstreamResponse.");
                return;
            }

            if (!ResponseDictionary.TryGetValue(downstreamRoute.Key, out var key)) return;

            if (TryParseJson(content, out var parsedContent))
            {
                lock (aggregatedResponse)
                {
                    aggregatedResponse[key] = parsedContent;
                }
            }
            else
            {
                logger.Error($"Error parsing JSON content for key {key}: {content}");
            }
        }
        else
        {
            logger.Warning("DownstreamRoute or DownstreamResponse not found in response items or invalid type.");
        }
    }

    private static bool TryParseJson(string content, out JToken? parsedContent)
    {
        try
        {
            parsedContent = JToken.Parse(content);
            return true;
        }
        catch (JsonReaderException)
        {
            parsedContent = null;
            return false;
        }
    }

    private static DownstreamResponse CreateDefaultDownstreamResponse()
    {
        return new DownstreamResponse(
            new StringContent("{}", Encoding.UTF8, "application/json"),
            HttpStatusCode.OK,
            new List<Header>(),
            string.Empty
        );
    }

    private static DownstreamResponse CreateAggregatedResponse(JObject aggregatedResponse,
        DownstreamResponse firstDownstreamResponse)
    {
        return new DownstreamResponse(
            new StringContent(aggregatedResponse.ToString(), Encoding.UTF8, "application/json"),
            firstDownstreamResponse.StatusCode,
            firstDownstreamResponse.Headers,
            firstDownstreamResponse.ReasonPhrase
        );
    }
}