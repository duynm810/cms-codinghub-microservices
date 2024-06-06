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
    : BaseController(errorService, logger)
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

                return HandleError((HttpStatusCode)posts.StatusCode, nameof(PostsByCategory));
            }

            return HandleError((HttpStatusCode)category.StatusCode, nameof(PostsByCategory));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(PostsByCategory));
        }
    }

    [HttpGet("post/{slug}")]
    public async Task<IActionResult> Details([FromRoute] string slug)
    {
        try
        {
            var posts = await postApiClient.GetPostBySlug(slug, 2);

            if (posts is { IsSuccess: true, Data: not null })
            {
                var items = new PostDetailViewModel()
                {
                    MainClass = "bg-grey pb-30",
                    Post = posts.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)posts.StatusCode, nameof(Details));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(Details));
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

            return HandleError((HttpStatusCode)posts.StatusCode, nameof(Search));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(Search));
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

                return HandleError((HttpStatusCode)postsInSeries.StatusCode, nameof(PostsInSeries));
            }

            return HandleError((HttpStatusCode)series.StatusCode, nameof(PostsInSeries));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(PostsInSeries));
        }
    }
}