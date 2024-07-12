using Shared.Enums;

namespace Shared.Dtos.Post.Queries;

public class SearchPostByCurrentUserDto
{
    public string? Keyword { get; set; }
    
    public PostStatusEnum? Status { get; set; }
    
    public Guid? UserId { get; set; }
}