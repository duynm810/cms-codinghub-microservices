using Microsoft.AspNetCore.Mvc;
using Shared.Requests.PostInSeries;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.PostInSeries;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("post-in-series")]
public class PostInSeriesController(IPostInSeriesApiClient postInSeriesApiClient) : BaseController
{
    [HttpPost("add-posts-to-series")]
    public async Task<IActionResult> AddPostsToSeries([FromBody] CreatePostInSeriesRequest request)
    {
        if (request.PostIds.Count == 0)
        {
            return Json(new { success = false, message = "No posts selected" });
        }

        var response = await postInSeriesApiClient.CreatePostsToSeries(request);
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