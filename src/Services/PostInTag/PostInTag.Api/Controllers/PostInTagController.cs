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
}