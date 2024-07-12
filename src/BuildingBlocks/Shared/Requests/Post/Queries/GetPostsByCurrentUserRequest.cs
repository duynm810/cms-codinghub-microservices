using Shared.Enums;
using Shared.Models.Common;

namespace Shared.Requests.Post.Queries;

public class GetPostsByCurrentUserRequest : Pagination
{
    public string? Keyword { get; set; }
    
    public PostStatusEnum? Status { get; set; }
    
    public Guid? UserId { get; set; }
}