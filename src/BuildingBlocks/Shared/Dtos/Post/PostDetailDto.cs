namespace Shared.Dtos.Post;

public class PostDetailDto
{
    public PostDto DetailPost { get; set; }
    
    public IEnumerable<PostDto> RelatedPosts { get; set; }
}