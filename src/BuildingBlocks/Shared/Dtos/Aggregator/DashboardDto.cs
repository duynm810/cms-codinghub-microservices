using Newtonsoft.Json;
using Shared.Dtos.Comment;
using Shared.Dtos.Post;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace Shared.Dtos.Aggregator;

public class DashboardDto
{
    [JsonProperty("featured-posts")] 
    public ApiResult<List<PostDto>> FeaturedPosts { get; set; } = new();

    [JsonProperty("most-liked-posts")] 
    public ApiResult<List<PostDto>> MostLikedPosts { get; set; } = new();

    [JsonProperty("pinned-posts")] 
    public ApiResult<List<PostDto>> PinnedPosts { get; set; } = new();

    [JsonProperty("latest-posts-paging")] 
    public ApiResult<PagedResponse<PostDto>> LatestPosts { get; set; } = new();

    [JsonProperty("suggest-tags")] 
    public ApiResult<List<TagDto>> SuggestTags { get; set; } = new();

    [JsonProperty("latest-comments")] 
    public ApiResult<List<CommentDto>> LatestComments { get; set; } = new();
}