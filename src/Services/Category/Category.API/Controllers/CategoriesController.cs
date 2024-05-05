using System.ComponentModel.DataAnnotations;
using System.Net;
using Category.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Category;

namespace Category.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var result = await categoryService.CreateCategory(categoryDto);
        return Ok(result);
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateCategory([FromRoute, Required] Guid id, [FromBody] UpdateCategoryDto categoryDto)
    {
        var result = await categoryService.UpdateCategory(id, categoryDto);
        return Ok(result);
    }
    
    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteCategory([FromRoute, Required] Guid[] ids)
    {
        var result = await categoryService.DeleteCategory(ids);
        return Ok(result);
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCategories()
    {
        var result = await categoryService.GetCategories();
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCategory([FromRoute, Required] Guid id)
    {
        var result = await categoryService.GetCategoryById(id);
        return Ok(result);
    }
}