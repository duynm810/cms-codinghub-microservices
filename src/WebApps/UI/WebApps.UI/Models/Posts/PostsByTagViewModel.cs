using Shared.Dtos.Post;
using Shared.Dtos.Tag;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostsByTagViewModel: BaseViewModel
{
    public TagDto Tag { get; set; } = default!;

    public PagedResponse<PostDto> Posts { get; set; } = default!;
}