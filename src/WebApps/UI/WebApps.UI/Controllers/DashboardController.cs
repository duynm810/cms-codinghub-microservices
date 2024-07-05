using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class DashboardController(
    IDashboardApiClient dashboardApiClient,
    IErrorService errorService,
    ILogger logger) : BaseController(errorService, logger)
{
    public async Task<IActionResult> Index()
    {
        const string methodName = nameof(Index);
        
        try
        {
            var data = await dashboardApiClient.GetDashboard();
    
            var viewModel = new DashboardViewModel();

            if (data.FeaturedPosts.Data is { Count: > 0 } featuredPosts)
            {
                viewModel.FeaturedPosts = featuredPosts;
            }

            if (data.PinnedPosts.Data is { Count: > 0 } pinnedPosts)
            {
                viewModel.PinnedPosts = pinnedPosts;
            }

            if (data.LatestPosts.Data is { Items: not null } latestPosts)
            {
                viewModel.LatestPosts = latestPosts;
            }

            if (data.MostLikedPosts.Data is { Count: > 0 } mostLikedPosts)
            {
                viewModel.MostLikedPosts = mostLikedPosts;
            }

            if (data.SuggestTags.Data is { Count: > 0 } suggestTags)
            {
                viewModel.SuggestTags = suggestTags;
            }

            return View(viewModel);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
}