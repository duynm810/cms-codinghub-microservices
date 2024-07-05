using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace Shared.Dtos.Dashboard;

public class DashboardDto
{
    [JsonProperty("featured-posts")]
    public FeaturedPosts FeaturedPosts { get; set; } = default!;
    
    [JsonProperty("most-liked-posts")]
    public MostLikedPosts MostLikedPosts { get; set; } = default!;

    [JsonProperty("pinned-posts")]
    public PinnedPosts PinnedPosts { get; set; } = default!;

    [JsonProperty("latest-posts-paging")]
    public LatestPosts LatestPosts { get; set; } = default!;
    
    [JsonProperty("suggest-tags")]
    public SuggestTags SuggestTags { get; set; } = default!;
}

public class FeaturedPosts : ApiResult<List<PostDto>>;

public class MostLikedPosts : ApiResult<List<PostDto>>;

public class PinnedPosts :  ApiResult<List<PostDto>>;

public class LatestPosts : ApiResult<PagedResponse<PostDto>>;

public class SuggestTags : ApiResult<List<TagDto>>;