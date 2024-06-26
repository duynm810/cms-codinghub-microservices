using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace WebApps.UI.Models.Accounts;

public class ManagePostsViewModel
{
    public PagedResponse<PostDto> Posts { get; set; } = default!;
}