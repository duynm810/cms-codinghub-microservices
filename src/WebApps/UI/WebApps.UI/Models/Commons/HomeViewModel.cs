using Shared.Dtos.Comment;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace WebApps.UI.Models.Commons;

public class HomeViewModel
{
    public List<PostDto> FeaturedPosts { get; set; } = default!;

    public IEnumerable<PostDto> PinnedPosts { get; set; } = default!;

    public PagedResponse<PostDto> LatestPosts { get; set; } = default!;

    public List<PostDto> MostLikedPosts { get; set; } = default!;
    
    public List<TagDto> SuggestTags { get; set; } = default!;
    
    public List<CommentDto> LatestComments { get; set; } = default!;
}