using Shared.Dtos.Category;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace WebApps.UI.Models.Posts;

public class PostsByAuthorViewModel
{
    public PostsByAuthorDto PostsByAuthor { get; set; } = default!;
}