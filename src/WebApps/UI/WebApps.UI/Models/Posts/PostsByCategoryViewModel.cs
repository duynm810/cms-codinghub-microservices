using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostsByCategoryViewModel
{
    public PostsByCategoryDto Datas { get; set; } = default!;
}