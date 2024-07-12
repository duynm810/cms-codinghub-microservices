using Shared.Enums;
using Shared.Models;

namespace Shared.Dtos.Post.Queries;

public class GetPostsByCurrentUserDto : Pagination
{
    public string? Keyword { get; set; }
    
    public PostStatusEnum? Status { get; set; }
    
    public Guid? UserId { get; set; }
}