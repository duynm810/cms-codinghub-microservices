using Shared.Dtos.Post;
using Shared.Dtos.Tag;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostDetailViewModel : BaseViewModel
{
    public PostDetailDto Post { get; set; } = default!;
}