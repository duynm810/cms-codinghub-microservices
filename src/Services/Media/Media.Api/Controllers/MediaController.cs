using Media.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Media.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [HttpPost("upload-image")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadImage(IFormFile? file, string type)
    {
        var result = await mediaService.UploadImage(file, type);
        return Ok(result);
    }
}