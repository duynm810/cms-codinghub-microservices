using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostDetailViewModel
{
    public PostBySlugDto Post { get; set; } = default!;
}