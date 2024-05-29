using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(IPostApiClient postApiClient, ILogger logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        var featuredPosts = await postApiClient.GetFeaturedPosts();
        if (featuredPosts is { IsSuccess: true, Data: not null })
        {
            var items = new HomeViewModel
            {
                FeaturedPosts = featuredPosts.Data
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