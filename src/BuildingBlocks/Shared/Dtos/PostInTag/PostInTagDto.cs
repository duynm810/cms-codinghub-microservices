using Shared.Dtos.Tag;
using Shared.Responses;

namespace Shared.Dtos.PostInTag;

public class PostInTagDto
{
    public TagDto Tag { get; set; } = default!;

    public PagedResponse<PostInTagDto> Posts { set; get; } = default!;
}