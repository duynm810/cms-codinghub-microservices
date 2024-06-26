namespace Shared.Dtos.Post.Queries;

public class PostsBySlugDto
{
    public PostDto Detail { get; set; } = default!;

    public IEnumerable<PostDto> RelatedPosts { get; set; } = default!;
}