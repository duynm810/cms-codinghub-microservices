using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Comment;
using Shared.Requests.Post.Queries;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Models.Posts;

namespace WebApps.UI.Controllers;

public class PostsController(
    IPostApiClient postApiClient)
    : BaseController
{
    [HttpGet("category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
    {
        const string methodName = nameof(PostsByCategory);

        try
        {
            var request = new GetPostsByCategoryRequest { PageNumber = page };
            var response = await postApiClient.GetPostsByCategoryPaging(categorySlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByCategoryViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return View();
    }

    [HttpGet("series/{seriesSlug}")]
    public async Task<IActionResult> PostsBySeries(string seriesSlug, int page = 1)
    {
        const string methodName = nameof(PostsBySeries);

        try
        {
            var request = new GetPostsBySeriesRequest { PageNumber = page };
            var response = await postApiClient.GetPostsBySeriesPaging(seriesSlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInSeriesViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }

            return View("Error", new ErrorViewModel() { StatusCode = response.StatusCode });
        }
        catch (Exception e)
        {
            return View("Error", new ErrorViewModel() { StatusCode = 500, StatusMessage = e.Message });
        }
    }

    [HttpGet("tag/{tagSlug}")]
    public async Task<IActionResult> PostsByTag(string tagSlug, int page = 1)
    {
        const string methodName = nameof(PostsByTag);

        try
        {
            var request = new GetPostsByTagRequest { PageNumber = page };
            var response = await postApiClient.GetPostsByTagPaging(tagSlug, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsInTagViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return View();
    }

    [HttpGet("author/{userName}")]
    public async Task<IActionResult> PostsByAuthor([FromRoute] string userName, [FromQuery] int page = 1)
    {
        const string methodName = nameof(PostsByAuthor);

        try
        {
            var request = new GetPostsByAuthorRequest { PageNumber = page };
            var response = await postApiClient.GetPostsByAuthorPaging(userName, request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostsByAuthorViewModel
                {
                    Datas = response.Data
                };

                return View(items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return View();
    }

    [HttpGet("post/{slug}")]
    public async Task<IActionResult> Details([FromRoute] string slug)
    {
        const string methodName = nameof(Details);

        try
        {
            var response = await postApiClient.GetDetailBySlug(slug, 2);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostDetailViewModel()
                {
                    Posts = response.Data
                };

                return View(items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return View();
    }

    public async Task<IActionResult> Search(string keyword, int page = 1)
    {
        const string methodName = nameof(Search);

        try
        {
            var request = new GetPostsRequest { PageNumber = page };
            var response = await postApiClient.SearchPostsPaging(request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new PostSearchViewModel()
                {
                    Keyword = keyword,
                    Posts = response.Data
                };

                return View(items);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        return View();
    }
}