using IdentityServer4.AccessTokenValidation;
using Media.Api.Dtos;
using Media.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Media.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] SingleFileDto request)
    {
        var result = await mediaService.UploadImage(request.File, request.Type);
        return Ok(result);
    }
}