using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.Models.Posts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(IPostApiClient postApiClient, ICategoryApiClient categoryApiClient, ISeriesApiClient seriesApiClient, PaginationSettings paginationSettings, ILogger logger) : Controller
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        var pageSize = paginationSettings.FeaturedPostPageSize;

        var category = await categoryApiClient.GetCategoryBySlug(categorySlug);
        var posts = await postApiClient.GetPostsByCategoryPaging(categorySlug, page, pageSize);
        
        if (posts is { IsSuccess: true, Data: not null } && category is { IsSuccess: true, Data: not null })
        {
            var items = new PostsByCategoryViewModel
            {
                Category = category.Data,
                Posts = posts.Data
            };
            
            return View(items);
        }
        
        logger.Error("Failed to load posts.");
        return Content("No posts.");
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
        
        logger.Error("Failed to load posts.");
        return Content("No posts.");
    }

    public async Task<IActionResult> Search(string keyword, int page = 1)
    {
        var pageSize = paginationSettings.SearchPostPageSize;

        var posts = await postApiClient.SearchPostsPaging(keyword, page, pageSize);
        
        if (posts is { IsSuccess: true, Data: not null })
        {
            var items = new PostSearchViewModel()
            {
                Keyword = keyword,
                Posts = posts.Data
            };

            return View(items);
        }

        logger.Error("Failed to load posts.");
        return Content("No posts.");
    }

    [HttpGet("series/{slug}")]
    public async Task<IActionResult> PostsInSeries(string slug, int page = 1)
    {
        var pageSize = paginationSettings.PostInSeriesPageSize;

        var series = await seriesApiClient.GetSeriesBySlug(slug);
        var postsInSeries = await postApiClient.GetPostsInSeriesBySlugPaging(slug, page, pageSize);
        
        if (series is { IsSuccess: true, Data: not null } && postsInSeries is { IsSuccess: true, Data: not null })
        {
            var items = new PostsInSeriesViewModel()
            {
                Series = series.Data,
                PostInSeries = postsInSeries.Data
            };

            return View(items);
        }
        
        return View();
    }
}