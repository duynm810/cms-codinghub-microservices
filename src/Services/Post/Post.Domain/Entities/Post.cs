using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Post.Domain.Entities;

[Table("Posts")]
[Index(nameof(Slug), IsUnique = true)]
public class Post : EntityAuditBase<Guid>
{
    /// <summary>
    /// Tiêu đề bài viết
    /// </summary>
    [Required]
    [MaxLength(250)]
    public required string Name { get; set; }
    
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
    public string? Description { get; set; }
    
    /// <summary>
    /// URL ảnh thu nhỏ đại diện cho bài viết
    /// </summary>
    [MaxLength(500)]
    public string? Thumbnail { get; set; }
    
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
    /// Tags cho bài viết
    /// </summary>
    [MaxLength(250)]
    public string? Tags { get; set; }

    /// <summary>
    /// Số lượt xem bài viết
    /// </summary>
    public int ViewCount { get; set; }
    
    /// <summary>
    /// Trạng thái bài viết này đã được thanh toán cho người đăng hay chưa
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Số tiền hoa hồng kiếm được từ bài viết nếu có
    /// </summary>
    public double RoyaltyAmount  { get; set; }
    
    /// <summary>
    /// Trạng thái của bài viết, ví dụ: Nháp, Chờ duyệt,..
    /// </summary>
    public PostStatusEnum Status { get; set; }
    
    /// <summary>
    /// Khóa ngoại đến danh mục của bài viết
    /// </summary>
    [Required]
    public Guid CategoryId { get; set; }
    
    /// <summary>
    /// Khóa ngoại đến thông tin tác giả bài viết
    /// </summary>
    [Required]
    public Guid AuthorUserId { get; set; }
    
    /// <summary>
    /// Ngày thanh toán cho bài viết, nếu là bài viết do người khác đăng
    /// </summary>
    public DateTime? PaidDate { get; set; }
}