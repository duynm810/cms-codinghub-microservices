using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(
    IDashboardApiClient dashboardApiClient,
    IPostApiClient postApiClient,
    IRazorRenderViewService razorRenderViewService,
    IErrorService errorService,
    ILogger logger) : BaseController(errorService, logger)
{
    public async Task<IActionResult> Index(int page = 1)
    {
        const string methodName = nameof(Index);
        
        try
        {
            var data = await dashboardApiClient.GetDashboard();
    
            var viewModel = new HomeViewModel();

            if (data.FeaturedPosts.Data is { Count: > 0 } featuredPosts)
            {
                viewModel.FeaturedPosts = featuredPosts;
            }

            if (data.PinnedPosts.Data is { Count: > 0 } pinnedPosts)
            {
                viewModel.PinnedPosts = pinnedPosts;
            }

            if (data.MostLikedPosts.Data is { Count: > 0 } mostLikedPosts)
            {
                viewModel.MostLikedPosts = mostLikedPosts;
            }

            if (data.SuggestTags.Data is { Count: > 0 } suggestTags)
            {
                viewModel.SuggestTags = suggestTags;
            }
            
            var latestPosts = await postApiClient.GetLatestPostsPaging(page, 6);
            if (latestPosts is { IsSuccess: true, Data: not null })
            {
                viewModel.LatestPosts = latestPosts.Data;
            }

            return View(viewModel);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
    
    public async Task<IActionResult> LatestPosts(int page = 1)
    {
        const string methodName = nameof(LatestPosts);

        try
        {
            var posts = await postApiClient.GetLatestPostsPaging(page, 6);
            if (posts is { IsSuccess: true, Data: not null })
            {
                var viewModel = new HomeViewModel { LatestPosts = posts.Data };
                var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Home/_LatestPosts.cshtml", viewModel);
                return Json(new { success = true, html });
            }
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }

        return Json(new { success = false });
    }

}