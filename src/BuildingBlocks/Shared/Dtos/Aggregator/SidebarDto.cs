using Newtonsoft.Json;
using Shared.Dtos.Comment;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Shared.Dtos.Aggregator;

public class SidebarDto
{
    [JsonProperty("most-commented-posts")] 
    public ApiResult<List<PostDto>> Posts { get; set; } = new();

    [JsonProperty("latest-comments")] 
    public ApiResult<List<LatestCommentDto>> LatestComments { get; set; } = new();
}