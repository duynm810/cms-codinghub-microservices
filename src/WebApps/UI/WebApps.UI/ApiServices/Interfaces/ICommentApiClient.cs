using Shared.Dtos.Comment;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface ICommentApiClient
{
    Task<ApiResult<List<CommentDto>>> GetCommentsByPostId(Guid postId);
}