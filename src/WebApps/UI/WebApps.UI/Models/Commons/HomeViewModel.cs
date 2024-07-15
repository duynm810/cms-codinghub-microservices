using Shared.Dtos.Comment;
using Shared.Dtos.Post;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace WebApps.UI.Models.Commons;

public class HomeViewModel
{
    public List<PostDto> FeaturedPosts { get; set; } = [];

    public List<PostDto> PinnedPosts { get; set; } = [];

    public PagedResponse<PostDto> LatestPosts { get; set; } = default!;

    public List<PostDto> MostLikedPosts { get; set; } = [];
    
    public List<TagDto> SuggestTags { get; set; } = [];
}