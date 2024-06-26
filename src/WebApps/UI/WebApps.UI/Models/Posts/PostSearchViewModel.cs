using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostSearchViewModel
{
    public PagedResponse<PostDto> Posts { set; get; } = default!;

    public string Keyword { set; get; } = default!;
}