using Shared.Dtos.Identity.User;
using Shared.Responses;

namespace Shared.Dtos.Post.Queries;

public class PostsByAuthorDto
{
    public UserDto User { get; set; } = default!;

    public PagedResponse<PostDto> Posts { get; set; } = default!;
}