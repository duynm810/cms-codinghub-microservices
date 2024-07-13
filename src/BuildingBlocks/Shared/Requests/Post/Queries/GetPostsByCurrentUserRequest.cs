using Shared.Enums;
using Shared.Models.Common;

namespace Shared.Requests.Post.Queries;

public class GetPostsByCurrentUserRequest : Pagination
{
    public int? Status { get; set; }
    
    public Guid? UserId { get; set; }
}