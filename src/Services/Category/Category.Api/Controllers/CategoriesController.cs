using System.ComponentModel.DataAnnotations;
using System.Net;
using Category.Api.Services.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Category;
using Shared.Enums;
using Shared.Responses;

namespace Category.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.Created)]
    [ClaimRequirement(FunctionCodeEnum.Category, CommandCodeEnum.Create)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var result = await categoryService.CreateCategory(categoryDto);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.NoContent)]
    [ClaimRequirement(FunctionCodeEnum.Category, CommandCodeEnum.Update)]
    public async Task<IActionResult> UpdateCategory([FromRoute, Required] long id,
        [FromBody] UpdateCategoryDto categoryDto)
    {
        var result = await categoryService.UpdateCategory(id, categoryDto);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    [ClaimRequirement(FunctionCodeEnum.Category, CommandCodeEnum.Delete)]
    public async Task<IActionResult> DeleteCategory([FromQuery, Required] List<long> ids)
    {
        var result = await categoryService.DeleteCategory(ids);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<CategoryDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var result = await categoryService.GetCategories();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategory([FromRoute, Required] long id)
    {
        var result = await categoryService.GetCategoryById(id);
        return Ok(result);
    }

    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<CategoryDto>>), (int)HttpStatusCode.OK)]
    [ClaimRequirement(FunctionCodeEnum.Category, CommandCodeEnum.View)]
    public async Task<IActionResult> GetCategoriesPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await categoryService.GetCategoriesPaging(pageNumber, pageSize);
        return Ok(result);
    }
    
    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<CategoryDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryBySlug([FromRoute, Required] string slug)
    {
        var result = await categoryService.GetCategoryBySlug(slug);
        return Ok(result);
    }
}