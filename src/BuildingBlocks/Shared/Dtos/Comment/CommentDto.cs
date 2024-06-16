using Shared.Dtos.Identity.User;
using Shared.Enums;

namespace Shared.Dtos.Comment;

public class CommentDto
{
    public string Id { get; set; }
    
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public string? Content { get; set; }

    public string? ParentId { get; set; }

    public int? Likes { get; set; }

    public int RepliesCount { get; set; }
    
    public DateTime CreatedDate { get; set; }

    public CommentStatusEnum Status { get; set; }
    
    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public UserDto? User { get; set; }
    
    public List<CommentDto>? Replies { get; set; }
}