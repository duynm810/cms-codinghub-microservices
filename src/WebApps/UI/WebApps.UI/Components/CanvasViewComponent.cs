using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class CanvasViewComponent(ISeriesApiClient seriesApiClient, ILogger logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var series = await seriesApiClient.GetSeries();
            if (series is { IsSuccess: true, Data: not null })
            {
                var items = new CanvasViewModel
                {
                    Series = series.Data
                };

                return View(items);
            }
            
            return Content("No categories found.");
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            throw;
        }
    }
}