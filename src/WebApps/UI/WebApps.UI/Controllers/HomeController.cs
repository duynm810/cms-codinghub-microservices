using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Requests.Post.Queries;
using Shared.Settings;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(
    IAggregatorApiClient aggregatorApiClient,
    IPostApiClient postApiClient,
    IRazorRenderViewService razorRenderViewService, ILogger logger) : BaseController(logger)
{
    public async Task<IActionResult> Index(int page = 1)
    {
        try
        {
            var response = await aggregatorApiClient.GetDashboard();
    
            var viewModel = new HomeViewModel();

            if (response.FeaturedPosts.Data is { Count: > 0 } featuredPosts)
            {
                viewModel.FeaturedPosts = featuredPosts;
            }

            if (response.PinnedPosts.Data is { Count: > 0 } pinnedPosts)
            {
                viewModel.PinnedPosts = pinnedPosts;
            }

            if (response.MostLikedPosts.Data is { Count: > 0 } mostLikedPosts)
            {
                viewModel.MostLikedPosts = mostLikedPosts;
            }

            if (response.SuggestTags.Data is { Count: > 0 } suggestTags)
            {
                viewModel.SuggestTags = suggestTags;
            }

            var latestPosts = await postApiClient.GetLatestPostsPaging(new GetLatestPostsRequest { PageNumber = page, PageSize = 4 });
            if (latestPosts is { IsSuccess: true, Data: not null })
            {
                viewModel.LatestPosts = latestPosts.Data;
            }
            
            return View(viewModel);
        }
        catch (Exception e)
        {
            return HandleException(nameof(Index), e);
        }
    }
    
    public async Task<IActionResult> LatestPosts(int page = 1)
    {
        try
        {
            var request = new GetLatestPostsRequest { PageNumber = page, PageSize = 4 };
            var response = await postApiClient.GetLatestPostsPaging(request);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return Json(new { success = false });
            }
            
            var viewModel = new HomeViewModel { LatestPosts = response.Data };
            var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Home/_LatestPosts.cshtml", viewModel);
            return Json(new { success = true, html });
        }
        catch (Exception e)
        {
            return HandleException(nameof(LatestPosts), e);
        }
    }
}