using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostInTag.Api.Services.Interfaces;
using Shared.Dtos.PostInTag;
using Shared.Requests.PostInTag;

namespace PostInTag.Api.Controllers;

[ApiController]
[Route("api/post-in-tag")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostInTagController(IPostInTagService postInTagService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePostToTag(CreatePostInTagRequest request)
    {
        var result = await postInTagService.CreatePostToTag(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePostToTag(DeletePostInTagRequest request)
    {
        var result = await postInTagService.DeletePostToTag(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}