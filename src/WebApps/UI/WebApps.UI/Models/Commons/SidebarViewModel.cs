using Shared.Dtos.Comment;
using Shared.Dtos.Post;

namespace WebApps.UI.Models.Commons;

public class SidebarViewModel
{
    public List<PostDto> Posts { get; set; } = [];
    
    public List<LatestCommentDto> LatestComments { get; set; } = [];
}