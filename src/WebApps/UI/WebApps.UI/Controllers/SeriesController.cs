using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Series;

namespace WebApps.UI.Controllers;

public class SeriesController(ISeriesApiClient seriesApiClient) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetSeries()
    {
        try
        {
            var response = await seriesApiClient.GetSeries();
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new SeriesViewModel()
                {
                    Series = response.Data
                };
                
                return PartialView("Partials/Accounts/_AddPostsToSeriesPartial", items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return Json(new { success = false });
    }
}