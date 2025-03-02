using System.Net;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostInSeries.Api.Services.Interfaces;
using Shared.Dtos.PostInSeries;
using Shared.Requests.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Controllers;

[ApiController]
[Route("api/post-in-series")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostInSeriesController(IPostInSeriesService postInSeriesService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePostToSeries(CreatePostInSeriesRequest request)
    {
        var result = await postInSeriesService.CreatePostToSeries(request);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePostToSeries([FromQuery] Guid postId, [FromQuery] Guid seriesId)
    {
        var result = await postInSeriesService.DeletePostToSeries(postId, seriesId);
        return Ok(result);
    }
    
    [HttpGet("{postId:guid}/manage-series")]
    [ProducesResponseType(typeof(ApiResult<ManagePostInSeriesDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSeriesForPost(Guid postId)
    {
        var result = await postInSeriesService.GetSeriesForPost(postId);
        return Ok(result);
    }
}