using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Post.Domain.Entities;

[Table("Posts")]
[Index(nameof(Slug), IsUnique = true)]
public class PostBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Tiêu đề bài viết
    /// </summary>
    [Required]
    [MaxLength(250)]
    public required string Title { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) bài viết
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(250)")]
    public required string Slug { get; set; }

    /// <summary>
    /// Nội dung chính bài viết
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Mô tả ngắn gọn bài viết
    /// </summary>
    [MaxLength(500)]
    public string? Summary { get; set; }

    /// <summary>
    /// URL ảnh thu nhỏ đại diện cho bài viết
    /// </summary>
    [MaxLength(500)]
    public string? Thumbnail { get; set; }
    
    /// <summary>
    /// ID của tệp Google Drive
    /// </summary>
    [MaxLength(100)]
    public string? ThumbnailFileId { get; set; }

    /// <summary>
    /// Mô tả SEO, dùng cho meta description
    /// </summary>
    [MaxLength(150)]
    public string? SeoDescription { get; set; }

    /// <summary>
    /// Nguồn bài viết nếu có (ví dụ: báo, tạp chí)
    /// </summary>
    [MaxLength(120)]
    public string? Source { get; set; }

    /// <summary>
    /// Số lượt xem bài viết
    /// </summary>
    [DefaultValue(0)]
    public int ViewCount { get; set; }

    /// <summary>
    /// Số lượng bình luận bài viết
    /// </summary>
    [DefaultValue(0)]
    public int CommentCount { get; set; }

    /// <summary>
    /// Số lượt thích bài viết
    /// </summary>
    [DefaultValue(0)]
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
    [Required]
    public long CategoryId { get; set; }

    /// <summary>
    /// ID của tác giả bài viết
    /// </summary>
    [Required]
    public Guid AuthorUserId { get; set; }

    /// <summary>
    /// Ngày thanh toán cho bài viết, nếu là bài viết do người khác đăng
    /// </summary>
    public DateTimeOffset? PaidDate { get; set; }
}