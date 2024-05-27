using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class PostsByCategoryViewModel
{
    public PagedResponse<PostByCategoryDto> Posts { get; set; }
}