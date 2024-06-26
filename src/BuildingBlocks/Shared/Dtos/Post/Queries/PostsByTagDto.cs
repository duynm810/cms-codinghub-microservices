using Shared.Dtos.Tag;
using Shared.Responses;

namespace Shared.Dtos.Post.Queries;

public class PostsByTagDto
{
    public TagDto Tag { get; set; } = default!;

    public PagedResponse<PostDto> Posts { get; set; } = default!;
}