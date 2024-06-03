using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SidebarViewComponent(IPostApiClient postApiClient, ILogger logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var posts = await postApiClient.GetMostCommentedPosts();
            if (posts is { IsSuccess: true, Data: not null })
            {
                var items = new SidebarViewModel
                {
                    Posts = posts.Data
                };

                return View(items);
            }
            
            return Content("No categories found.");
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            throw;
        }
    }
}