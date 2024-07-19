using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class TagsController(ITagApiClient tagApiClient, ILogger logger) : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> GetSuggestedTags([FromQuery] int count = 5)
    {
        try
        {
            var response = await tagApiClient.GetSuggestedTags(count);
            return response is not { IsSuccess: true, Data: not null }
                ? Json(new { success = false })
                : Json(new { success = true, data = response.Data });
        }
        catch (Exception e)
        {
            return HandleException(nameof(GetSuggestedTags), e);
        }
    }
}