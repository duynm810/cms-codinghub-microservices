using System.ComponentModel.DataAnnotations;
using System.Net;
using Comment.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Comment;
using Shared.Extensions;
using Shared.Responses;

namespace Comment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<CommentDto>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto request)
    {
        request.UserId = User.GetUserId();
        
        var result = await commentService.CreateComment(request);
        return Ok(result);
    }

    [Route("by-post/{postId:guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<CommentDto>>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId([Required] Guid postId)
    {
        var result = await commentService.GetCommentsByPostId(postId);
        return Ok(result);
    }
    
    [Route("latest")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<CommentDto>>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetLatestComments([FromQuery] int count)
    {
        var result = await commentService.GetLatestComments(count);
        return Ok(result);
    }
    
    [Route("like/{commentId}")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> LikeComment(string commentId)
    {
        var result = await commentService.LikeComment(commentId);
        return Ok(result);
    }
    
    [Route("reply")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ReplyToComment([FromQuery, Required] string parentId, [FromBody] CreateCommentDto request)
    {
        var result = await commentService.ReplyToComment(parentId, request);
        return Ok(result);
    }
}