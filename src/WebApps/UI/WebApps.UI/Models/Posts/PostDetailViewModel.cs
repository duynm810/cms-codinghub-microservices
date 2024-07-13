using Shared.Dtos.Post;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostDetailViewModel
{
    public PostsBySlugDto Posts { get; set; } = default!;
}