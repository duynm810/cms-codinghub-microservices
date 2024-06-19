using Shared.Dtos.Identity.User;
using Shared.Dtos.Tag;
using Shared.Enums;

namespace Shared.Dtos.Post;

public class PostDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? Content { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }
    
    public int ViewCount { get; set; }

    public int CommentCount { get; set; }

    public int LikeCount { get; set; }

    public bool IsPinned { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsPaid { get; set; }

    public DateTimeOffset? PublishedDate { get; set; }
    
    public PostStatusEnum Status { get; set; }

    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public string? CategorySeoDescription { get; set; }

    public string? CategoryIcon { get; set; }

    public string? CategoryColor { get; set; }

    public Guid AuthorUserId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
    
    /// <summary>
    /// List of tags in the article (Danh sách các thẻ thuộc bài viết)
    /// </summary>
    public List<TagDto>? Tags { get; set; }
    
    /// <summary>
    /// Article author information (Thông tin tác giả bài viết)
    /// </summary>
    public UserDto? User { get; set; }
}