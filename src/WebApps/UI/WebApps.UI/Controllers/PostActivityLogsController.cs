using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.PostActivityLogs;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostActivityLogsController(IPostActivityLogApiClient postActivityLogApiClient) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetPostActivityLogs([FromRoute] Guid postId)
    {
        const string methodName = nameof(GetPostActivityLogs);

        try
        {
            var response = await postActivityLogApiClient.GetActivityLogs(postId);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostActivityLogsViewModel()
                {
                    PostActivityLogs = response.Data
                };
                
                return PartialView("Partials/Accounts/_PostActivityLogsPartial", items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return Json(new { success = false });
    }
}