using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.Models;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(IPostApiClient postApiClient, PaginationSettings paginationSettings, ILogger logger) : Controller
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        var pageSize = paginationSettings.PageSize;
        
        var posts = await postApiClient.GetPostsByCategory(categorySlug, page, pageSize);
        if (posts is { IsSuccess: true, Data: not null })
        {
            var items = new PostsByCategoryViewModel
            {
                Posts = posts.Data
            };
            
            return View(items);
        }
        
        logger.Error("Failed to load posts by category slug.");
        return Content("No posts by category slug.");
    }
}