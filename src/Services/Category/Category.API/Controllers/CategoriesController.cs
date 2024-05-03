using System.Net;
using Category.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Category.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCategories()
    {
        var result = await categoryService.GetCategories();
        return Ok(result);
    }
}