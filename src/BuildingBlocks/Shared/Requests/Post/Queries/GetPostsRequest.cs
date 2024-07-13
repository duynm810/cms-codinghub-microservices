using Shared.Models.Common;

namespace Shared.Requests.Post.Queries;

public class GetPostsRequest : Pagination
{
    public string? Filter { get; set; }
}