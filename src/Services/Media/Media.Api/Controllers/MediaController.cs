using IdentityServer4.AccessTokenValidation;
using Media.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Media.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile? file, string type)
    {
        var result = await mediaService.UploadImage(file, type);
        return Ok(result);
    }
}