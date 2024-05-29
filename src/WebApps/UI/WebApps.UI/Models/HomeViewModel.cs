using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class HomeViewModel
{
    public required IEnumerable<PostDto> FeaturedPosts { get; set; }
    
    public required PagedResponse<PostDto> LatestPosts { get; set; }
}