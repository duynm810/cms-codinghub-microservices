namespace Shared.Dtos.Comment;

public class CreateCommentDto
{
    public Guid UserId { get; set; }
    
    public Guid PostId { get; set; }

    public string? Content { get; set; }

    public string? ParentId { get; set; }
}