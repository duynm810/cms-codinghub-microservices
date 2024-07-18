using Microsoft.AspNetCore.Mvc;
using Shared.Requests.PostInSeries;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.PostInSeries;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("post-in-series")]
public class PostInSeriesController(IPostInSeriesApiClient postInSeriesApiClient) : BaseController
{
    [HttpPost("add-post-to-series")]
    public async Task<IActionResult> AddPostToSeries([FromBody] CreatePostInSeriesRequest request)
    {
        var response = await postInSeriesApiClient.CreatePostToSeries(request);
        return Json(response.IsSuccess ? new { success = true } : new { success = false });
    }
    
    [HttpDelete("delete-post-from-series")]
    public async Task<IActionResult> DeletePostFromSeries([FromQuery] Guid postId, [FromQuery] Guid seriesId)
    {
        var response = await postInSeriesApiClient.DeletePostToSeries(postId, seriesId);
        return Json(response.IsSuccess ? new { success = true } : new { success = false });
    }

    [HttpGet("{postId:guid}/manage-series")]
    public async Task<IActionResult> GetSeriesForPost([FromRoute] Guid postId)
    {
        var response = await postInSeriesApiClient.GetSeriesForPost(postId);
        if (response is { IsSuccess: true, Data: not null })
        {
            var items = new ManageSeriesViewModel()
            {
                Series = response.Data.Series,
                CurrentSeries = response.Data.CurrentSeries
            };
            
            return PartialView("Partials/Accounts/_ManageSeriesPartial", items);
        }
        
        return Json(new { success = false });
    }
}