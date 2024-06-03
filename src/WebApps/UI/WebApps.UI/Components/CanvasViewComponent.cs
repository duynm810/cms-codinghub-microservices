using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class CanvasViewComponent(ISeriesApiClient seriesApiClient, IErrorService errorService, ILogger logger) : BaseViewComponent(errorService)
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
            
            return HandleError((HttpStatusCode)series.StatusCode);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(InvokeAsync), e);
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}