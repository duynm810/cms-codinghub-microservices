using Comment.Api.Entities;

namespace Comment.Api.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<bool> CreateComment(CommentBase comment);
    
    Task<List<CommentBase>> GetCommentsByPostId(Guid postId);

    Task<List<CommentBase>> GetLatestComments(int count);

    Task<CommentBase?> GetCommentById(string id);

    Task<bool> UpdateLikeCount(string commentId, int increment);

    Task<bool> UpdateRepliesCount(string parentId, int increment);
}