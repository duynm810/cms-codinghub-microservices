using Shared.Dtos.Comment;
using Shared.Responses;

namespace Comment.Api.Services.Interfaces;

public interface ICommentService
{
    Task<ApiResult<CommentDto>> CreateComment(CreateCommentDto model);
    
    Task<ApiResult<IEnumerable<CommentDto>>> GetCommentsByPostId(Guid postId);
}