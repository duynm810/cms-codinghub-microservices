using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Features.V1.PostActivityLogs.Queries.GetPostActivityLogs;
using Shared.Dtos.PostActivity;

namespace Post.Api.Controllers;

[Route("api/posts")]
[ApiController]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostActivityLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet("activity-logs/{postId:guid}")]
    public async Task<ActionResult<List<PostActivityLogDto>>> GetActivityLogs(Guid postId)
    {
        var query = new GetPostActivityLogsQuery(postId);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}