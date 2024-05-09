using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Commons.Models;
using Post.Application.Features.V1.Posts.Commands.CreatePost;
using Post.Application.Features.V1.Posts.Commands.DeletePost;
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
using Post.Application.Features.V1.Posts.Queries.GetPostById;
using Post.Application.Features.V1.Posts.Queries.GetPosts;
using Post.Application.Features.V1.Posts.Queries.GetPostsPaging;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreatePost([FromBody] CreateOrUpdatePostDto model)
    {
        var command = mapper.Map<CreatePostCommand>(model);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PostDto>> UpdatePost([Required] Guid id, [FromBody] UpdatePostCommand command)
    {
        command.SetId(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeletePost([Required] Guid id)
    {
        var command = new DeletePostCommand(id);
        await mediator.Send(command);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPosts()
    {
        var query = new GetPostsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PostDto>> GetPostById([Required] Guid id)
    {
        var query = new GetPostByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPostsPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsPagingQuery(pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}