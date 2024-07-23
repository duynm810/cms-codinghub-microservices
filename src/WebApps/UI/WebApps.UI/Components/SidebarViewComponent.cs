using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SidebarViewComponent(IAggregatorApiClient aggregatorApiClient, ILogger logger)
    : BaseViewComponent(logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        const string methodName = nameof(InvokeAsync);

        try
        {
            var items = new SidebarViewModel();

            var response = await aggregatorApiClient.GetSidebar();

            if (response.Posts.Data is { Count: > 0 } posts)
            {
                items.Posts = posts;
            }

            if (response.LatestComments.Data is { Count: > 0 } latestComments)
            {
                items.LatestComments = latestComments;
            }

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }
}