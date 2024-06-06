using System.ComponentModel.DataAnnotations;
using System.Net;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Series.Api.Services.Interfaces;
using Shared.Dtos.Series;
using Shared.Responses;

namespace Series.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class SeriesController(ISeriesService seriesService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<SeriesDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateSeries([FromBody] CreateSeriesDto seriesDto)
    {
        var result = await seriesService.CreateSeries(seriesDto);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<SeriesDto>), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateSeries([FromRoute, Required] Guid id,
        [FromBody] UpdateSeriesDto seriesDto)
    {
        var result = await seriesService.UpdateSeries(id, seriesDto);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteSeries([FromQuery, Required] List<Guid> ids)
    {
        var result = await seriesService.DeleteSeries(ids);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<SeriesDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeries([FromQuery] int count = 4)
    {
        var result = await seriesService.GetSeries(count);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<SeriesDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSeries([FromRoute, Required] Guid id)
    {
        var result = await seriesService.GetSeriesById(id);
        return Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<SeriesDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeriesBySlug([FromRoute, Required] string slug)
    {
        var result = await seriesService.GetSeriesBySlug(slug);
        return Ok(result);
    }

    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<SeriesDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSeriesPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var result = await seriesService.GetSeriesPaging(pageNumber, pageSize);
        return Ok(result);
    }
}