using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class HeaderViewComponent(ICategoryApiClient categoryApiClient, ILogger logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
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
}