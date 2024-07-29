namespace Shared.Dtos.Post;

public class PostsBySlugDto
{
    public PostDto Detail { get; set; } = default!;

    public List<PostDto> RelatedPosts { get; set; } = [];
}