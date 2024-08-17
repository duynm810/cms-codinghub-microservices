using IdentityServer4.AccessTokenValidation;
using Media.Api.Dtos;
using Media.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Media.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
/*[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]*/
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] SingleFileDto request)
    {
        var result = await mediaService.UploadImage(request);
        return Ok(result);
    }

    [HttpDelete("delete-image/{imagePath}")]
    public IActionResult DeleteImage([FromRoute] string imagePath)
    {
        var result = mediaService.DeleteImage(imagePath);
        return Ok(result);
    }
    
    [HttpPost("upload-image-to-google-drive")]
    public async Task<IActionResult> UploadImageToGoogleDrive([FromForm] SingleFileDto request)
    {
        var result = await mediaService.UploadImageToGoogleDrive(request);
        return Ok(result);
    }

    [HttpDelete("delete-image-from-google-drive/{imagePath}")]
    public IActionResult DeleteImageFromGoogleDrive([FromRoute] string imagePath)
    {
        var result = mediaService.DeleteImageFromGoogleDrive(imagePath);
        return Ok(result);
    }
}