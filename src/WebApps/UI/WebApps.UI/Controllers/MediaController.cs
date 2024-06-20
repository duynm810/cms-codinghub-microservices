using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController(IMediaApiClient mediaApiClient) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string type)
    {
        var result = await mediaApiClient.UploadImage(file, type);
        return Ok(result);
    }
}