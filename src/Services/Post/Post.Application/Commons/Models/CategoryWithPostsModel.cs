namespace Post.Application.Commons.Models;

public class CategoryWithPostsModel
{
    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public List<PostModel>? Posts { get; set; }
}