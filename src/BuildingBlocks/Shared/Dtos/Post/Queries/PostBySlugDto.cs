namespace Shared.Dtos.Post.Queries;

public class PostBySlugDto
{
    public PostDto Detail { get; set; } = default!;

    public IEnumerable<PostDto> RelatedPosts { get; set; } = default!;
}