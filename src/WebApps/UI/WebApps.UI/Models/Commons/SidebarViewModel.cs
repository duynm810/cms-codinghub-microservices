using Shared.Dtos.Comment;
using Shared.Dtos.Post.Queries;

namespace WebApps.UI.Models.Commons;

public class SidebarViewModel
{
    public List<PostDto> Posts { get; set; } = default!;
    
    public List<LatestCommentDto> LatestComments { get; set; } = default!;
}