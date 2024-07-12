using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SidebarViewComponent(IPostApiClient postApiClient, ICommentApiClient commentApiClient, IErrorService errorService, ILogger logger)
    : BaseViewComponent(errorService, logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var posts = await postApiClient.GetMostCommentedPosts(6);
            var latestComments = await commentApiClient.GetLatestComments(4);
            var items = new SidebarViewModel
            {
                Posts = posts.Data,
                LatestComments = latestComments.Data
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}