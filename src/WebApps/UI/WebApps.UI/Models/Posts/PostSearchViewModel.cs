using Shared.Dtos.Post;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostSearchViewModel : BaseViewModel
{
    public PagedResponse<PostDto> Posts { set; get; } = default!;

    public string Keyword { set; get; } = default!;
}