using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.Models;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(IPostApiClient postApiClient, ICategoryApiClient categoryApiClient, PaginationSettings paginationSettings, ILogger logger) : Controller
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        var pageSize = paginationSettings.FeaturedPostPageSize;

        var category = await categoryApiClient.GetCategoryBySlug(categorySlug);
        var posts = await postApiClient.GetPostsByCategory(categorySlug, page, pageSize);
        if (posts is { IsSuccess: true, Data: not null } && category is { IsSuccess: true, Data: not null })
        {
            var items = new PostsByCategoryViewModel
            {
                Category = category.Data,
                Posts = posts.Data
            };
            
            return View(items);
        }
        
        logger.Error("Failed to load posts by category slug.");
        return Content("No posts by category slug.");
    }

    [HttpGet("post/{slug}")]
    public async Task<IActionResult> Details([FromRoute] string slug)
    {
        var posts = await postApiClient.GetPostBySlug(slug);
        if (posts is { IsSuccess: true, Data: not null })
        {
            var items = new PostDetailViewModel()
            {
                Post = posts.Data
            };
            
            return View(items);
        }
        
        logger.Error("Failed to load post detail.");
        return Content("No post detail.");
    }
}