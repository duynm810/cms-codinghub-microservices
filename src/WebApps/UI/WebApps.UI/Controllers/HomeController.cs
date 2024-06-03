using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(IPostApiClient postApiClient, PaginationSettings paginationSettings, ILogger logger) : Controller
{
    public async Task<IActionResult> Index(int page = 1)
    {
        var pageSize = paginationSettings.LatestPostPageSize;
        
        var featuredPosts = await postApiClient.GetFeaturedPosts();
        var pinnedPosts = await postApiClient.GetPinnedPosts();
        var latestPosts = await postApiClient.GetLatestPostsPaging(page, pageSize);
        var mostLikedPosts = await postApiClient.GetMostLikedPosts();
        
        if (featuredPosts is { IsSuccess: true, Data: not null } && 
            pinnedPosts is { IsSuccess: true, Data: not null } && 
            latestPosts is { IsSuccess: true, Data: not null } &&
            mostLikedPosts is { IsSuccess: true, Data: not null })
        {
            var items = new HomeViewModel
            {
                FeaturedPosts = featuredPosts.Data,
                PinnedPosts = featuredPosts.Data,
                LatestPosts = latestPosts.Data,
                MostLikedPosts = mostLikedPosts.Data
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
}