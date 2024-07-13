using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostInSeries.Api.Services.Interfaces;
using Shared.Dtos.PostInSeries;
using Shared.Requests.PostInSeries;

namespace PostInSeries.Api.Controllers;

[ApiController]
[Route("api/post-in-series")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostInSeriesController(IPostInSeriesService postInSeriesService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePostToSeries(CreatePostInSeriesRequest request)
    {
        var result = await postInSeriesService.CreatePostToSeries(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePostToSeries(DeletePostInSeriesRequest request)
    {
        var result = await postInSeriesService.DeletePostToSeries(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}