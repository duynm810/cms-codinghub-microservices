using Shared.Dtos.Post;
using Shared.Dtos.Series;
using Shared.Responses;

namespace WebApps.UI.Models.Commons;

public class HomeViewModel
{
    public IEnumerable<PostDto> FeaturedPosts { get; set; } = default!;

    public PagedResponse<PostDto> LatestPosts { get; set; } = default!;
    
    public IEnumerable<SeriesDto> Series { get; set; } = default!;
}