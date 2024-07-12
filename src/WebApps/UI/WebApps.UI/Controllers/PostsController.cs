using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Comment;
using Shared.Requests.Post.Queries;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Posts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class PostsController(
    IPostApiClient postApiClient,
    ICommentApiClient commentApiClient,
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
            var request = new GetPostsByCategoryRequest { PageNumber = page };
            var response =await postApiClient.GetPostsByCategoryPaging(categorySlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByCategoryViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var request = new GetPostsBySeriesRequest { PageNumber = page };
            var response =await postApiClient.GetPostsBySeriesPaging(seriesSlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInSeriesViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var request = new GetPostsByTagRequest { PageNumber = page };
            var response =await postApiClient.GetPostsByTagPaging(tagSlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInTagViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var request = new GetPostsByAuthorRequest { PageNumber = page };
            var response =await postApiClient.GetPostsByAuthorPaging(userName, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByAuthorViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var response =await postApiClient.GetDetailBySlug(slug, 2);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostDetailViewModel()
                {
                    Posts = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var request = new GetPostsRequest { PageNumber = page };
            var response =await postApiClient.SearchPostsPaging(request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostSearchViewModel()
                {
                    Keyword = keyword,
                    Posts = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
        return Ok(new { data = newComment.Data });
    }

    [HttpPost]
    public async Task<IActionResult> ReplyToComment([FromQuery] string parentId, [FromBody] CreateCommentDto comment)
    {
        var replyToComment = await commentApiClient.ReplyToComment(parentId, comment);
        return Ok(new { data = replyToComment.Data });
    }
}