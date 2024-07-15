using Shared.Dtos.Post;
using Shared.Dtos.PostActivity;
using Shared.Responses;

namespace WebApps.UI.Models.Accounts;

public class ManagePostsViewModel
{
    public PagedResponse<PostDto> Posts { get; set; } = default!;
    
    public List<PostActivityLogDto> PostActivityLogs { get; set; } = default!;
}