using Microsoft.AspNetCore.Mvc;
using Shared.Requests.PostInSeries;
using WebApps.UI.ApiClients.Interfaces;

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

        var result = await postInSeriesApiClient.CreatePostsToSeries(request);
        return Json(result.IsSuccess ? new { success = true } : new { success = false });
    }
}