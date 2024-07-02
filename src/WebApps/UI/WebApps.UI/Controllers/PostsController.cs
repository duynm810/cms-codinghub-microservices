using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Comment;
using Shared.Settings;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Posts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(
    IPostApiClient postApiClient,
    ICommentApiClient commentApiClient,
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

            var result = await postApiClient.GetPostsByCategoryPaging(categorySlug, page, pageSize);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByCategoryViewModel
                {
                    Datas = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
    
    [HttpGet("series/{seriesSlug}")]
    public async Task<IActionResult> PostsBySeries(string seriesSlug, int page = 1)
    {
        const string methodName = nameof(PostsBySeries);

        try
        {
            var pageSize = paginationSettings.PostInSeriesPageSize;

            var result = await postApiClient.GetPostsBySeriesPaging(seriesSlug, page, pageSize);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInSeriesViewModel
                {
                    Datas = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet("tag/{tagSlug}")]
    public async Task<IActionResult> PostsByTag(string tagSlug, int page = 1)
    {
        const string methodName = nameof(PostsByTag);

        try
        {
            var pageSize = paginationSettings.PostInSeriesPageSize;

            var result = await postApiClient.GetPostsByTagPaging(tagSlug, page, pageSize);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInTagViewModel
                {
                    Datas = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
    
    [HttpGet("author/{userName}")]
    public async Task<IActionResult> PostsByAuthor([FromRoute] string userName, [FromQuery] int page = 1)
    {
        const string methodName = nameof(PostsByAuthor);

        try
        {
            var pageSize = paginationSettings.FeaturedPostPageSize;

            var result = await postApiClient.GetPostsByAuthorPaging(userName, page, pageSize);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByAuthorViewModel
                {
                    Datas = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
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
            var result = await postApiClient.GetDetailBySlug(slug, 2);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostDetailViewModel()
                {
                    Posts = result.Data
                };
                
                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
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

            var result = await postApiClient.SearchPostsPaging(keyword, page, pageSize);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new PostSearchViewModel()
                {
                    Keyword = keyword,
                    Posts = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
    
    public async Task<IActionResult> GetCommentsByPostId([FromQuery] Guid postId)
    {
        var comments = await commentApiClient.GetCommentsByPostId(postId);
        return Ok(new { data = comments.Data });
    }
    
    [HttpPost]
    public async Task<IActionResult> AddNewComment([FromBody] CreateCommentDto comment)
    {
        var newComment = await commentApiClient.CreateComment(comment);
        return Ok(new { data = newComment });
    }
}