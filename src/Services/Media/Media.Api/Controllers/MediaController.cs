using Media.Api.Dtos;
using Media.Api.Services.Interfaces;
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

    [HttpDelete("delete-image-from-google-drive/{fileId}")]
    public async Task<IActionResult> DeleteImageFromGoogleDrive([FromRoute] string fileId)
    {
        var result = await mediaService.DeleteImageFromGoogleDrive(fileId);
        return Ok(result);
    }
    
    [HttpGet("get-image/{fileId}")]
    public async Task<IActionResult> GetImage([FromRoute] string fileId)
    {
        var result = await mediaService.GetImage(fileId);
        if (result.Data == null)
        {
            return NotFound("File not found.");
        }
        
        return File(result.Data, "image/jpeg");
    }
}