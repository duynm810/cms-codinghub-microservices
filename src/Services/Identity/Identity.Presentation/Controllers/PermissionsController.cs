using System.Net;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.Permission;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PermissionsController(IIdentityReposityManager identityReposityManager) : ControllerBase
{
    [HttpPost("roles/{roleId}")]
    [ProducesResponseType(typeof(PermissionDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] CreatePermissionDto model)
    {
        var result = await identityReposityManager.Permissions.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
    }
}