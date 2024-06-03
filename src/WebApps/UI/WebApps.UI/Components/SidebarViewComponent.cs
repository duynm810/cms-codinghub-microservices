using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SidebarViewComponent(IPostApiClient postApiClient, IErrorService errorService, ILogger logger)
    : BaseViewComponent(errorService)
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

            return HandleError((HttpStatusCode)posts.StatusCode);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}