using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Commons.Models;
using Post.Application.Features.V1.PostActivityLogs.Queries.GetPostActivityLogs;

namespace Post.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostActivityLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{postId:guid}")]
    public async Task<ActionResult<List<PostActivityLogModel>>> GetActivityLogs(Guid postId)
    {
        var query = new GetPostActivityLogsQuery(postId);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}