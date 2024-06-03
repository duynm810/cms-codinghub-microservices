using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class BottomViewComponent(IPostApiClient postApiClient, IErrorService errorService, ILogger logger) : BaseViewComponent(errorService)
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
            return HandleError((HttpStatusCode)postsByNonStaticPageCategory.StatusCode);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}