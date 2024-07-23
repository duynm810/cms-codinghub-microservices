using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Comment;
using WebApps.UI.ApiClients.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public class CommentsController(ICommentApiClient commentApiClient, ILogger logger) : BaseController(logger)
{
    public async Task<IActionResult> GetCommentsByPostId([FromQuery] Guid postId)
    {
        try
        {
            var comments = await commentApiClient.GetCommentsByPostId(postId);
            return Ok(new { data = comments.Data });
        }
        catch (Exception e)
        {
            return HandleException(nameof(GetCommentsByPostId), e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddNewComment([FromBody] CreateCommentRequest comment)
    {
        try
        {
            var newComment = await commentApiClient.CreateComment(comment);
            return Ok(new { data = newComment.Data });
        }
        catch (Exception e)
        {
            return HandleException(nameof(AddNewComment), e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReplyToComment([FromQuery] string parentId, [FromBody] CreateCommentRequest comment)
    {
        try
        {
            var replyToComment = await commentApiClient.ReplyToComment(parentId, comment);
            return Ok(new { data = replyToComment.Data });
        }
        catch (Exception e)
        {
            return HandleException(nameof(AddNewComment), e);
        }
    }
}