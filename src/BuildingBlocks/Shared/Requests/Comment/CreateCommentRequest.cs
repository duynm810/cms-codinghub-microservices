namespace Shared.Requests.Comment;

public class CreateCommentRequest
{
    public Guid UserId { get; set; }
    
    public Guid PostId { get; set; }

    public string? Content { get; set; }

    public string? ParentId { get; set; }
}