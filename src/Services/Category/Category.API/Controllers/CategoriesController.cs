using System.ComponentModel.DataAnnotations;
using System.Net;
using Category.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var result = await categoryService.CreateCategory(categoryDto);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategory([FromRoute, Required] Guid id,
        [FromBody] UpdateCategoryDto categoryDto)
    {
        var result = await categoryService.UpdateCategory(id, categoryDto);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategory([FromRoute, Required] Guid[] ids)
    {
        var result = await categoryService.DeleteCategory(ids);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<CategoryDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategories()
    {
        var result = await categoryService.GetCategories();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategory([FromRoute, Required] Guid id)
    {
        var result = await categoryService.GetCategoryById(id);
        return Ok(result);
    }

    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<CategoryDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoriesPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await categoryService.GetCategoriesPaging(pageNumber, pageSize);
        return Ok(result);
    }
}