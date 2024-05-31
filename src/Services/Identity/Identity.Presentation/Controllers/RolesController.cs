using System.Net;
using Identity.Infrastructure.Services.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.Role;
using Shared.Responses;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<RoleDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateRole([FromBody] CreateOrUpdateRoleDto request)
    {
        var result = await roleService.CreateRole(request);
        return Ok(result);
    }

    [HttpPut("{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] CreateOrUpdateRoleDto request)
    {
        var result = await roleService.UpdateRole(roleId, request);
        return Ok(result);
    }

    [HttpDelete("{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteRole(Guid roleId)
    {
        var result = await roleService.DeleteRole(roleId);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<RoleDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetRoles()
    {
        var result = await roleService.GetRoles();
        return Ok(result);
    }

    [HttpGet("{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResult<RoleDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetRoleById(Guid roleId)
    {
        var result = await roleService.GetRoleById(roleId);
        return Ok(result);
    }
}
