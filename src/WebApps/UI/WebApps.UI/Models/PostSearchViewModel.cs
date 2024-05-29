using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class PostSearchViewModel
{
    public PagedResponse<PostDto> Posts { set; get; } = default!;

    public string Keyword { set; get; } = default!;
}