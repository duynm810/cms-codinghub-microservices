using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController(IMediaApiClient mediaApiClient) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string type)
    {
        var result = await mediaApiClient.UploadImage(file, type);
        return Ok(result);
    }

    [HttpDelete("delete-image/{imagePath}")]
    public async Task<IActionResult> DeleteImage([FromRoute] string imagePath)
    {
        var result = await mediaApiClient.DeleteImage(imagePath);
        return Ok(result);
    }
    
    [HttpPost("upload-image-from-google-drive")]
    public async Task<IActionResult> UploadImageToGoogleDrive([FromForm] IFormFile file, [FromForm] string type)
    {
        var result = await mediaApiClient.UploadImageToGoogleDrive(file, type);
        return Ok(result);
    }

    [HttpDelete("delete-image-from-google-drive/{fileId}")]
    public async Task<IActionResult> DeleteImageFromGoogleDrive([FromRoute] string fileId)
    {
        var result = await mediaApiClient.DeleteImageFromGoogleDrive(fileId);
        return Ok(result);
    }
    
    [HttpGet("get-image")]
    public async Task<IActionResult> GetImage([FromRoute] string fileId)
    {
        var result = await mediaApiClient.GetImage(fileId);
        return Ok(result);
    }
}