using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SidebarViewComponent(IPostApiClient postApiClient, IErrorService errorService, ILogger logger)
    : BaseViewComponent(errorService, logger)
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

            return HandleError((HttpStatusCode)posts.StatusCode, nameof(InvokeAsync));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}