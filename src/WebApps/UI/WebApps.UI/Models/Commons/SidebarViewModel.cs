using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;

namespace WebApps.UI.Models.Commons;

public class SidebarViewModel
{
    public IEnumerable<PostDto> Posts { get; set; } = default!;
}