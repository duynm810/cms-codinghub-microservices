using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("[controller]")]
public class SeriesController(ISeriesApiClient seriesApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSeries()
    {
        var response = await seriesApiClient.GetSeries();
        return Ok(response);
    }
}