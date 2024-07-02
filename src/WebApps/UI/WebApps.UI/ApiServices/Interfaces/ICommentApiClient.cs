using Shared.Dtos.Comment;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface ICommentApiClient
{
    Task<ApiResult<List<CommentDto>>> GetCommentsByPostId(Guid postId);

    Task<ApiResult<CommentDto>> CreateComment(CreateCommentDto comment);

    Task<ApiResult<CommentDto>> ReplyToComment(string parentId, CreateCommentDto comment);
}