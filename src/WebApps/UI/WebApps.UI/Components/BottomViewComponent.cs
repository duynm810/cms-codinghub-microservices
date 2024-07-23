using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class BottomViewComponent(IPostApiClient postApiClient, ILogger logger)
    : BaseViewComponent(logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        const string methodName = nameof(InvokeAsync);
        
        try
        {
            var response = await postApiClient.GetPostsByNonStaticPageCategory(3);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return HandleError(methodName, response.StatusCode);
            }
            
            var items = new BottomViewModel
            {
                PostsWithCategory = response.Data
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }
}