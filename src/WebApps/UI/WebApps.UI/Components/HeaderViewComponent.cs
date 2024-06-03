using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class HeaderViewComponent(ICategoryApiClient categoryApiClient, ILogger logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var categories = await categoryApiClient.GetCategories();
            if (categories is { IsSuccess: true, Data: not null })
            {
                var viewModel = new HeaderViewModel
                {
                    Categories = categories.Data
                };

                return View(viewModel);
            }

            logger.Error("Failed to load categories or no categories found.");
            return Content("No categories found.");
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            return Content("Unable to load categories at this time.");
        }
    }
}