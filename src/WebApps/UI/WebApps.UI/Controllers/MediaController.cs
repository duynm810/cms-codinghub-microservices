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
}