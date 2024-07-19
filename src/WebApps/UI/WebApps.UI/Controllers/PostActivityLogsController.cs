using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.PostActivityLogs;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostActivityLogsController(IPostActivityLogApiClient postActivityLogApiClient, ILogger logger)
    : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> GetPostActivityLogs([FromRoute] Guid postId)
    {
        try
        {
            var response = await postActivityLogApiClient.GetActivityLogs(postId);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return Json(new { success = false });
            }

            var items = new PostActivityLogsViewModel()
            {
                PostActivityLogs = response.Data
            };

            return PartialView("Partials/Accounts/_PostActivityLogsPartial", items);
        }
        catch (Exception e)
        {
            return HandleException(nameof(GetPostActivityLogs), e);
        }
    }
}