using Shared.Dtos.Post;
using Shared.Dtos.Series;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace WebApps.UI.Models.Commons;

public class HomeViewModel
{
    public IEnumerable<PostDto> FeaturedPosts { get; set; } = default!;

    public IEnumerable<PostDto> PinnedPosts { get; set; } = default!;

    public PagedResponse<PostDto> LatestPosts { get; set; } = default!;

    public IEnumerable<PostDto> MostLikedPosts { get; set; } = default!;
    
    public IEnumerable<TagDto> Tags { get; set; } = default!;
}