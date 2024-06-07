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
    ITagApiClient tagApiClient,
    PaginationSettings paginationSettings,
    IErrorService errorService,
    ILogger logger)
    : BaseController(errorService, logger)
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        const string methodName = nameof(PostsByCategory);

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

                return HandleError((HttpStatusCode)posts.StatusCode, methodName);
            }

            return HandleError((HttpStatusCode)category.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet("post/{slug}")]
    public async Task<IActionResult> Details([FromRoute] string slug)
    {
        const string methodName = nameof(Details);

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

            return HandleError((HttpStatusCode)posts.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    public async Task<IActionResult> Search(string keyword, int page = 1)
    {
        const string methodName = nameof(Search);

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

            return HandleError((HttpStatusCode)posts.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet("series/{slug}")]
    public async Task<IActionResult> PostsInSeries(string slug, int page = 1)
    {
        const string methodName = nameof(PostsInSeries);

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

                return HandleError((HttpStatusCode)postsInSeries.StatusCode, methodName);
            }

            return HandleError((HttpStatusCode)series.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet("tag/{slug}")]
    public async Task<IActionResult> PostsInTag(string slug, int page = 1)
    {
        const string methodName = nameof(PostsInTag);

        try
        {
            var pageSize = paginationSettings.PostInSeriesPageSize;

            var tag = await tagApiClient.GetTagBySlug(slug);
            var postsInTag = await postApiClient.GetPostsInTagBySlugPaging(slug, page, pageSize);

            if (tag is { IsSuccess: true, Data: not null })
            {
                if (postsInTag is { IsSuccess: true, Data: not null })
                {
                    var items = new PostsInTagViewModel
                    {
                    };

                    return View(items);
                }

                return HandleError((HttpStatusCode)postsInTag.StatusCode, methodName);
            }

            return HandleError((HttpStatusCode)tag.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
}