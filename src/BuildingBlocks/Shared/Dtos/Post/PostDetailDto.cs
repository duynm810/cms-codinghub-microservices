namespace Shared.Dtos.Post;

public class PostDetailDto
{
    public PostDto DetailPost { get; set; } = default!;

    public IEnumerable<PostDto> RelatedPosts { get; set; } = default!;
}