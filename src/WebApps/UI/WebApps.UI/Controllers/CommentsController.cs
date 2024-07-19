using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Comment;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.Controllers;

public class CommentsController(ICommentApiClient commentApiClient) : BaseController
{
    public async Task<IActionResult> GetCommentsByPostId([FromQuery] Guid postId)
    {
        var comments = await commentApiClient.GetCommentsByPostId(postId);
        return Ok(new { data = comments.Data });
    }

    [HttpPost]
    public async Task<IActionResult> AddNewComment([FromBody] CreateCommentRequest comment)
    {
        var newComment = await commentApiClient.CreateComment(comment);
        return Ok(new { data = newComment.Data });
    }

    [HttpPost]
    public async Task<IActionResult> ReplyToComment([FromQuery] string parentId, [FromBody] CreateCommentRequest comment)
    {
        var replyToComment = await commentApiClient.ReplyToComment(parentId, comment);
        return Ok(new { data = replyToComment.Data });
    }
}