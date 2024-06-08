using Shared.Dtos.Post;
using Shared.Dtos.PostInTag;
using Shared.Dtos.Tag;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostsInTagViewModel: BaseViewModel
{
    public TagDto Tag { get; set; } = default!;

    public PagedResponse<PostInTagDto> PostInTags { set; get; } = default!;
}