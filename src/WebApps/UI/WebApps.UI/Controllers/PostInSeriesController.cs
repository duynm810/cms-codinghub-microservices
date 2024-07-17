using Microsoft.AspNetCore.Mvc;
using Shared.Requests.PostInSeries;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("post-in-series")]
public class PostInSeriesController : Controller
{
    [HttpPost("add-post-in-series")]
    public async Task<IActionResult> AddPostsToSeries([FromBody] CreatePostInSeriesRequest request)
    {
        if (!request.PostIds.Any())
        {
            return Json(new { success = false, message = "No posts selected" });
        }
        
        return Json(new { success = false });
    }
}