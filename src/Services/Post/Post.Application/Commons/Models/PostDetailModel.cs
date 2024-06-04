namespace Post.Application.Commons.Models;

public class PostDetailModel
{
    public PostModel? DetailPost { get; set; }

    public IEnumerable<PostModel>? RelatedPosts { get; set; }
}