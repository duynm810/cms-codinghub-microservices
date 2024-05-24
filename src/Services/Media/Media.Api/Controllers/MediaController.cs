using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Media.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController(IWebHostEnvironment hostEnvironment, IOptions<MediaSettings> mediaSettings) : ControllerBase
{
    private readonly MediaSettings _mediaSettings = mediaSettings.Value;
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> UploadImage(string type)
    {
        var baseImageFolder = _mediaSettings.ImageFolder ?? "DefaultImagePath";
        var allowImageTypes = _mediaSettings.AllowImageFileTypes?.Split(",");

        if (Request.Form.Files.Count == 0)
        {
            return BadRequest("No files have been uploaded.");
        }

        var file = Request.Form.Files[0];
        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');

        if (allowImageTypes?.Any(x => filename?.EndsWith(x, StringComparison.OrdinalIgnoreCase) == true) == false)
        {
            return BadRequest("File upload is not allowed.");
        }

        if (string.IsNullOrEmpty(filename))
        {
            return BadRequest("Invalid file name.");
        }

        // Cấu trúc thư mục: [Base Image Folder]/images/[Type]/[MMyyyy]
        var now = DateTime.Now;
        var imageFolder = Path.Combine(baseImageFolder, "images", type, now.ToString("MMyyyy"));
            
        if (string.IsNullOrWhiteSpace(hostEnvironment.WebRootPath))
        {
            hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        var folderPath = Path.Combine(hostEnvironment.WebRootPath, imageFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, filename);
        await using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fs);
        }

        var path = Path.Combine(imageFolder, filename).Replace("\\", "/");
        return Ok(new { path });
    }
}