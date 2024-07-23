using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class CanvasViewComponent(ISeriesApiClient seriesApiClient, ILogger logger)
    : BaseViewComponent(logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        const string methodName = nameof(InvokeAsync);

        try
        {
            var response = await seriesApiClient.GetSeries();
            if (response is not { IsSuccess: true, Data: not null })
            {
                return HandleError(methodName, response.StatusCode);
            }

            var items = new CanvasViewModel
            {
                Series = response.Data
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }
}