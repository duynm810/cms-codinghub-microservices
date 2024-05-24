using System.Net;
using Identity.Infrastructure.Services.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.User;
using Shared.Responses;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<UserDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
    {
        var result = await userService.CreateUser(model);
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto model)
    {
        var result = await userService.UpdateUser(userId, model);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await userService.DeleteUser(userId);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<UserDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await userService.GetUsers();
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var result = await userService.GetUserById(userId);
        return Ok(result);
    }
    
    [HttpPost("{userId:guid}/change-password")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordUserDto model)
    {
        var result = await userService.ChangePassword(userId, model);
        return Ok(result);
    }
}