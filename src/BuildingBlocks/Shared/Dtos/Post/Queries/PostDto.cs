using Shared.Dtos.Category;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Tag;
using Shared.Enums;

namespace Shared.Dtos.Post.Queries;

public class PostDto
{
    /// <summary>
    /// Khoá chính
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Tiêu đề bài viết
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) bài viết
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Nội dung chính bài viết
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Mô tả ngắn gọn bài viết
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// URL ảnh thu nhỏ đại diện cho bài viết
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Mô tả SEO, dùng cho meta description
    /// </summary>
    public string? SeoDescription { get; set; }

    /// <summary>
    /// Nguồn bài viết nếu có (ví dụ: báo, tạp chí)
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Số lượt xem bài viết
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Số lượng bình luận bài viết
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Số lượt thích bài viết
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// Bài viết có được ghim lên đầu không
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Bài viết có nổi bật hay không
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Bài viết này đã được thanh toán cho người đăng hay chưa
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Số tiền hoa hồng kiếm được từ bài viết nếu có
    /// </summary>
    public double RoyaltyAmount { get; set; }

    /// <summary>
    /// Trạng thái của bài viết, ví dụ: Nháp, Chờ duyệt,..
    /// </summary>
    public PostStatusEnum Status { get; set; }

    /// <summary>
    /// Ngày công bố bài viết
    /// </summary>
    public DateTimeOffset? PublishedDate { get; set; }

    /// <summary>
    /// Khóa ngoại đến danh mục của bài viết
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// ID của tác giả bài viết
    /// </summary>
    public Guid AuthorUserId { get; set; }

    /// <summary>
    /// Ngày thanh toán cho bài viết, nếu là bài viết do người khác đăng
    /// </summary>
    public DateTimeOffset? PaidDate { get; set; }
    
    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }
    
    /// <summary>
    /// Danh mục thuộc bài viết
    /// </summary>
    public CategoryDto Category { get; set; }

    /// <summary>
    /// Danh sách các thẻ thuộc bài viết
    /// </summary>
    public List<TagDto>? Tags { get; set; } = [];
    
    /// <summary>
    /// Thông tin tác giả bài viết
    /// </summary>
    public UserDto? User { get; set; }
}