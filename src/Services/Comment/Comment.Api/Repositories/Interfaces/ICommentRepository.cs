using Comment.Api.Entities;

namespace Comment.Api.Repositories.Interfaces;

public interface ICommentRepository
{
    Task CreateComment(CommentBase comment);
    
    Task<IEnumerable<CommentBase>> GetCommentsByPostId(Guid postId);
    
    
}