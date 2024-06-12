using System.ComponentModel.DataAnnotations;
using System.Net;
using Comment.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Comment;
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
        var result = await commentService.CreateComment(request);
        return Ok(result);
    }

    [Route("by-post/{postId:guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CommentDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId([Required] Guid postId)
    {
        var result = await commentService.GetCommentsByPostId(postId);
        return Ok(result);
    }
}