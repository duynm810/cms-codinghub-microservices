using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.Settings;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class HomeController(
    IPostApiClient postApiClient,
    ITagApiClient tagApiClient,
    PaginationSettings paginationSettings,
    IErrorService errorService,
    ILogger logger) : BaseController(errorService, logger)
{
    public async Task<IActionResult> Index(int page = 1)
    {
        const string methodName = nameof(Index);
        
        try
        {
            var pageSize = paginationSettings.LatestPostPageSize;

            var featuredPosts = await postApiClient.GetFeaturedPosts(4);
            var pinnedPosts = await postApiClient.GetPinnedPosts(4);
            var latestPosts = await postApiClient.GetLatestPostsPaging(page, pageSize);
            var mostLikedPosts = await postApiClient.GetMostLikedPosts(4);
            var tags = await tagApiClient.GetTags(4);

            if (featuredPosts is { IsSuccess: true, Data: not null } &&
                pinnedPosts is { IsSuccess: true, Data: not null } &&
                latestPosts is { IsSuccess: true, Data: not null } &&
                mostLikedPosts is { IsSuccess: true, Data: not null } &&
                tags is { IsSuccess: true, Data: not null })
            {
                var items = new HomeViewModel
                {
                    FeaturedPosts = featuredPosts.Data,
                    PinnedPosts = featuredPosts.Data,
                    LatestPosts = latestPosts.Data,
                    MostLikedPosts = mostLikedPosts.Data,
                    Tags = tags.Data
                };

                return View(items);
            }

            return HandleError(HttpStatusCode.NotFound, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }
}