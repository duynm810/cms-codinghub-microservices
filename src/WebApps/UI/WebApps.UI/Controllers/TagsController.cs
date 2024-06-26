using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(ITagApiClient tagApiClient) :  ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSuggestedTags([FromQuery] int count = 5)
    {
        var result = await tagApiClient.GetSuggestedTags(count);
        return Ok(result);
    }
}