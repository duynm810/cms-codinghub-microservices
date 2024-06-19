namespace Shared.Dtos.Post.Queries;

public class PostDetailDto
{
    public PostDto DetailPost { get; set; } = default!;

    public IEnumerable<PostDto> RelatedPosts { get; set; } = default!;
}