using System.ComponentModel.DataAnnotations;
using System.Net;
using Category.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.Api.Controllers;

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

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategory([FromRoute, Required] long id,
        [FromBody] UpdateCategoryDto categoryDto)
    {
        var result = await categoryService.UpdateCategory(id, categoryDto);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategory([FromQuery, Required] List<long> ids)
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

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategory([FromRoute, Required] long id)
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