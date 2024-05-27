using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Commons.Models;
using Post.Application.Features.V1.Posts.Commands.ApprovePost;
using Post.Application.Features.V1.Posts.Commands.CreatePost;
using Post.Application.Features.V1.Posts.Commands.DeletePost;
using Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;
using Post.Application.Features.V1.Posts.Commands.SubmitPostForApproval;
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
using Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;
using Post.Application.Features.V1.Posts.Queries.GetPostById;
using Post.Application.Features.V1.Posts.Queries.GetPosts;
using Post.Application.Features.V1.Posts.Queries.GetPostsPaging;
using Shared.Dtos.Post;
using Shared.Extensions;
using Shared.Responses;

namespace Post.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreatePost([FromBody] CreatePostDto model)
    {
        var command = mapper.Map<CreatePostCommand>(model);
        command.AuthorUserId = User.GetUserId();
        
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
        var result = await mediator.Send(command);
        return Ok(result);
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
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPostById([Required] Guid id)
    {
        var query = new GetPostByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsPagingQuery(pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedPosts(int count = 5)
    {
        var query = new GetFeaturedPostsQuery(count);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("approve/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ApprovePost(Guid id)
    {
        var command = new ApprovePostCommand(id, User.GetUserId());
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("submit-for-approval/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SubmitPostForApproval(Guid id)
    {
        var command = new SubmitPostForApprovalCommand(id, User.GetUserId());
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("reject/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RejectPostWithReasonCommand(Guid id, [FromBody] RejectPostWithReasonDto model)
    {
        var command = new RejectPostWithReasonCommand(id, User.GetUserId(), model);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}