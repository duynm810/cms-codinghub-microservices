using Shared.Dtos.Comment;
using Shared.Requests.Comment;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface ICommentApiClient
{
    Task<ApiResult<List<CommentDto>>> GetCommentsByPostId(Guid postId);
    
    Task<ApiResult<List<LatestCommentDto>>> GetLatestComments(int count);

    Task<ApiResult<CommentDto>> CreateComment(CreateCommentRequest request);

    Task<ApiResult<CommentDto>> ReplyToComment(string parentId, CreateCommentRequest request);
}