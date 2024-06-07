using System.ComponentModel.DataAnnotations;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostInTag.Api.Services.Interfaces;
using Shared.Dtos.PostInTag;

namespace PostInTag.Api.Controllers;

[ApiController]
[Route("api/post-in-tag")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostInTagController(IPostInTagService postInTagService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePostToTag(CreatePostInTagDto request)
    {
        var result = await postInTagService.CreatePostToTag(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePostToTag(DeletePostInTagDto request)
    {
        var result = await postInTagService.DeletePostToTag(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInTag(Guid tagId)
    {
        var result = await postInTagService.GetPostsInTag(tagId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("by-slug")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInTagBySlug([FromQuery, Required] string slug)
    {
        var result = await postInTagService.GetPostsInTagBySlug(slug);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInTagPaging(Guid tagId, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await postInTagService.GetPostsInTagPaging(tagId, pageNumber, pageSize);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("by-slug/{slug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostInTagBySlugPaging(string slug, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await postInTagService.GetPostsInTagBySlugPaging(slug, pageNumber, pageSize);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}