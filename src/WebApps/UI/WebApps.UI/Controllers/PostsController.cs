using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Posts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(
    IPostApiClient postApiClient,
    ICategoryApiClient categoryApiClient,
    ISeriesApiClient seriesApiClient,
    PaginationSettings paginationSettings,
    IErrorService errorService,
    ILogger logger)
    : BaseController(errorService)
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        try
        {
            var pageSize = paginationSettings.FeaturedPostPageSize;

            var category = await categoryApiClient.GetCategoryBySlug(categorySlug);
            var posts = await postApiClient.GetPostsByCategoryPaging(categorySlug, page, pageSize);

            if (category is { IsSuccess: true, Data: not null })
            {
                if (posts is { IsSuccess: true, Data: not null })
                {
                    var items = new PostsByCategoryViewModel
                    {
                        Category = category.Data,
                        Posts = posts.Data
                    };

                    return View(items);
                }

                logger.Error("Failed to load posts. Status code: {StatusCode}", posts.StatusCode);
                return HandleError((HttpStatusCode)posts.StatusCode);
            }

            logger.Error("Failed to load category. Status code: {StatusCode}", category.StatusCode);
            return HandleError((HttpStatusCode)category.StatusCode);
        }
        catch(Exception ex)
        {
            logger.Error(ex, "An error occurred while loading posts by category.");
            return HandleError(HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("post/{slug}")]
    public async Task<IActionResult> Details([FromRoute] string slug)
    {
        try
        {
            var posts = await postApiClient.GetPostBySlug(slug);

            if (posts is { IsSuccess: true, Data: not null })
            {
                var items = new PostDetailViewModel()
                {
                    MainClass = "bg-grey pb-30",
                    Post = posts.Data
                };

                return View(items);
            }

            logger.Error("Failed to load posts.");
            return HandleError((HttpStatusCode)posts.StatusCode);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while loading the post details.");
            return HandleError(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<IActionResult> Search(string keyword, int page = 1)
    {
        try
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
            return HandleError((HttpStatusCode)posts.StatusCode);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while searching posts.");
            return HandleError(HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("series/{slug}")]
    public async Task<IActionResult> PostsInSeries(string slug, int page = 1)
    {
        try
        {
            var pageSize = paginationSettings.PostInSeriesPageSize;

            var series = await seriesApiClient.GetSeriesBySlug(slug);
            var postsInSeries = await postApiClient.GetPostsInSeriesBySlugPaging(slug, page, pageSize);

            if (series is { IsSuccess: true, Data: not null })
            {
                if (postsInSeries is { IsSuccess: true, Data: not null })
                {
                    var items = new PostsInSeriesViewModel
                    {
                        Series = series.Data,
                        PostInSeries = postsInSeries.Data
                    };

                    return View(items);
                }

                logger.Error("Failed to load posts in series. Status code: {StatusCode}", postsInSeries.StatusCode);
                return HandleError((HttpStatusCode)postsInSeries.StatusCode);
            }

            logger.Error("Failed to load series. Status code: {StatusCode}", series.StatusCode);
            return HandleError((HttpStatusCode)series.StatusCode);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while loading posts in series.");
            return HandleError(HttpStatusCode.InternalServerError);
        }
    }
}