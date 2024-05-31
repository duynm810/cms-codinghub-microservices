using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(IPostApiClient postApiClient, ISeriesApiClient seriesApiClient, PaginationSettings paginationSettings, ILogger logger) : Controller
{
    public async Task<IActionResult> Index(int page = 1)
    {
        var pageSize = paginationSettings.LatestPostPageSize;
        
        var featuredPosts = await postApiClient.GetFeaturedPosts();
        var latestPosts = await postApiClient.GetLatestPostsPaging(page, pageSize);
        var series = await seriesApiClient.GetSeries();
        
        if (featuredPosts is { IsSuccess: true, Data: not null } && 
            latestPosts is { IsSuccess: true, Data: not null } &&
            series is { IsSuccess: true, Data: not null })
        {
            var items = new HomeViewModel
            {
                FeaturedPosts = featuredPosts.Data,
                LatestPosts = latestPosts.Data,
                Series = series.Data
            };

            return View(items);
        }
        
        logger.Error("Failed to load featured posts or no featured posts found.");
        return Content("No featured posts found.");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}