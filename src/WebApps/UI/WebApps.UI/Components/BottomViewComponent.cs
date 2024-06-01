using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class BottomViewComponent(IPostApiClient postApiClient, ILogger logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var postsByNonStaticPageCategory = await postApiClient.GetPostsByNonStaticPageCategory();
            if (postsByNonStaticPageCategory is { IsSuccess: true, Data: not null })
            {
                var viewModel = new BottomViewModel
                {
                    PostsWithCategory = postsByNonStaticPageCategory.Data
                };

                return View(viewModel);
            }

            logger.Error("Failed to load categories or no categories found.");
            return Content("No categories found.");
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            throw;
        }
    }
}