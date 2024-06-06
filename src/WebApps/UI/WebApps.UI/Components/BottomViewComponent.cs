using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class BottomViewComponent(IPostApiClient postApiClient, IErrorService errorService, ILogger logger)
    : BaseViewComponent(errorService, logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var postsByNonStaticPageCategory = await postApiClient.GetPostsByNonStaticPageCategory(3);
            if (postsByNonStaticPageCategory is { IsSuccess: true, Data: not null })
            {
                var viewModel = new BottomViewModel
                {
                    PostsWithCategory = postsByNonStaticPageCategory.Data
                };

                return View(viewModel);
            }

            return HandleError((HttpStatusCode)postsByNonStaticPageCategory.StatusCode, nameof(InvokeAsync));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}