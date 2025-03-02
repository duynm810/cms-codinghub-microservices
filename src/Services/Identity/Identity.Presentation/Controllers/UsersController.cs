using System.Net;
using Identity.Infrastructure.Services.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Identity.User;
using Shared.Extensions;
using Shared.Requests.Identity.User;
using Shared.Responses;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<UserDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await userService.CreateUser(request);
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var result = await userService.UpdateUser(userId, request);
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

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var result = await userService.GetUserById(userId);
        return Ok(result);
    }
    
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetMe()
    {
        var result = await userService.GetUserById(User.GetUserClaimId());
        return Ok(result);
    }

    [HttpPost("{userId:guid}/change-password")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordUserRequest request)
    {
        var result = await userService.ChangePassword(userId, request);
        return Ok(result);
    }
    
    [HttpPut("{userId:guid}/update-avatar")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAvatar(Guid userId, [FromBody] UpdateAvatarRequest request)
    {
        var result = await userService.UpdateAvatar(userId, request);
        return Ok(result);
    }
}