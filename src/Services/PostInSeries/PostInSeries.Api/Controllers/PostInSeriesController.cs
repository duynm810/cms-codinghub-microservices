using System.ComponentModel.DataAnnotations;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostInSeries.Api.Services.Interfaces;
using Shared.Dtos.PostInSeries;

namespace PostInSeries.Api.Controllers;

[ApiController]
[Route("api/post-in-series")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostInSeriesController(IPostInSeriesService postInSeriesService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePostToSeries(CreatePostInSeriesDto request)
    {
        var result = await postInSeriesService.CreatePostToSeries(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePostToSeries(DeletePostInSeriesDto request)
    {
        var result = await postInSeriesService.DeletePostToSeries(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInSeries(Guid seriesId)
    {
        var result = await postInSeriesService.GetPostsInSeries(seriesId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("by-slug")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInSeriesBySlug([FromQuery, Required] string slug)
    {
        var result = await postInSeriesService.GetPostsInSeriesBySlug(slug);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInSeriesPaging(Guid seriesId, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await postInSeriesService.GetPostsInSeriesPaging(seriesId, pageNumber, pageSize);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("by-slug/{slug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInSeriesBySlugPaging(string slug, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await postInSeriesService.GetPostsInSeriesBySlugPaging(slug, pageNumber, pageSize);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}