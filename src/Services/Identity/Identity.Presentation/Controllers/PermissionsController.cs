using System.ComponentModel.DataAnnotations;
using System.Net;
using Identity.Infrastructure.Services.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.Permission;
using Shared.Responses;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]/roles/{roleId}")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<PermissionDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] CreateOrUpdatePermissionDto model)
    {
        var result = await permissionService.CreatePermission(roleId, model);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> UpdatePermissions(string roleId,
        [FromBody] IEnumerable<CreateOrUpdatePermissionDto> permissions)
    {
        var result = await permissionService.UpdatePermissions(roleId, permissions);
        return Ok(result);
    }

    [HttpDelete("{function}/{command}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> DeletePermission(string roleId, [Required] string function,
        [Required] string command)
    {
        var result = await permissionService.DeletePermission(roleId, function, command);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<PermissionDto>>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var result = await permissionService.GetPermissions(roleId);
        return Ok(result);
    }
}