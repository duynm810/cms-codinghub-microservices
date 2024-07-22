using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class SearchViewComponent(ILogger logger) : BaseViewComponent(logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        const string methodName = nameof(InvokeAsync);

        try
        {
            var items = new SearchViewModel
            {
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }
}