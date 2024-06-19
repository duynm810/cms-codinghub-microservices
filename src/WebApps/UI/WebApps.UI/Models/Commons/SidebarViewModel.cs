using Shared.Dtos.Post;

namespace WebApps.UI.Models.Commons;

public class SidebarViewModel
{
    public IEnumerable<PostDto> Posts { get; set; } = default!;
}