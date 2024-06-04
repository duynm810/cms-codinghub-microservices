using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SearchViewComponent(IErrorService errorService, ILogger logger) : BaseViewComponent(errorService, logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var items = new SearchViewModel
            {
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}