using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(ITagApiClient tagApiClient) :  ControllerBase
{
    [HttpGet("suggest")]
    public async Task<IActionResult> GetSuggestedTags([FromQuery] int count = 5)
    {
        var response = await tagApiClient.GetSuggestedTags(count);
        return Ok(response);
    }
}