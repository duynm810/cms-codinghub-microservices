using Microsoft.AspNetCore.Mvc;
using Shared.Requests.PostInSeries;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.PostInSeries;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("post-in-series")]
public class PostInSeriesController(IPostInSeriesApiClient postInSeriesApiClient, ILogger logger)
    : BaseController(logger)
{
    [HttpPost("add-post-to-series")]
    public async Task<IActionResult> AddPostToSeries([FromBody] CreatePostInSeriesRequest request)
    {
        try
        {
            var response = await postInSeriesApiClient.CreatePostToSeries(request);
            return Json(response.IsSuccess ? new { success = true } : new { success = false });
        }
        catch (Exception e)
        {
            return HandleException(nameof(AddPostToSeries), e);
        }
    }

    [HttpDelete("delete-post-from-series")]
    public async Task<IActionResult> DeletePostFromSeries([FromQuery] Guid postId, [FromQuery] Guid seriesId)
    {
        try
        {
            var response = await postInSeriesApiClient.DeletePostToSeries(postId, seriesId);
            return Json(response.IsSuccess ? new { success = true } : new { success = false });
        }
        catch (Exception e)
        {
            return HandleException(nameof(DeletePostFromSeries), e);
        }
    }

    [HttpGet("{postId:guid}/manage-series")]
    public async Task<IActionResult> GetSeriesForPost([FromRoute] Guid postId)
    {
        try
        {
            var response = await postInSeriesApiClient.GetSeriesForPost(postId);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return Json(new { success = false });
            }

            var items = new ManageSeriesViewModel()
            {
                Series = response.Data.Series,
                CurrentSeries = response.Data.CurrentSeries
            };

            return PartialView("Partials/Accounts/_ManageSeriesPartial", items);
        }
        catch (Exception e)
        {
            return HandleException(nameof(GetSeriesForPost), e);
        }
    }
}