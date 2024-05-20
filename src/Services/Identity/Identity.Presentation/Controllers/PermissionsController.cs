using System.ComponentModel.DataAnnotations;
using System.Net;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.Permission;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]/roles/{roleId}")]
[AllowAnonymous]
public class PermissionsController(IIdentityReposityManager identityReposityManager) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PermissionDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] CreateOrUpdatePermissionDto model)
    {
        var result = await identityReposityManager.Permissions.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
    }

    [HttpPut]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdatePermissions(string roleId,
        [FromBody] IEnumerable<CreateOrUpdatePermissionDto> permissions)
    {
        await identityReposityManager.Permissions.UpdatePermissions(roleId, permissions);
        return NoContent();
    }

    [HttpDelete("{function}/{command}")]
    [ProducesResponseType(typeof(PermissionDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePermission(string roleId, [Required] string function,
        [Required] string command)
    {
        await identityReposityManager.Permissions.DeletePermission(roleId, function, command);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var result = await identityReposityManager.Permissions.GetPermissions(roleId);
        return Ok(result);
    }
}