using Shared.Dtos.Identity.User;

namespace Shared.Dtos.Comment;

public class LatestCommentDto
{
    public required string Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid PostId { get; set; }
    
    public string? PostSlug { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public UserDto? User { get; set; }
}