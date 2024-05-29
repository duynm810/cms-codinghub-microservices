using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class HomeViewModel
{
    public IEnumerable<PostDto> FeaturedPosts { get; set; } = default!;

    public PagedResponse<PostDto> LatestPosts { get; set; } = default!;
}