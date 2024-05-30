using Contracts.Domains;

namespace Series.Grpc.Entities;

public class SeriesBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Tên của loạt bài viết
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) của loạt bài viết
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Mô tả ngắn gọn loạt bài viết
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) của mô tả
    /// </summary>
    public string? SeoDescription { get; set; }

    /// <summary>
    /// URL ảnh thu nhỏ đại diện cho loạt bài viết
    /// </summary>
    public string? Thumbnail { set; get; }

    /// <summary>
    /// Nội dung chính loạt bài viết
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Khóa ngoại đến thông tin tác giả bài viết
    /// </summary>
    public Guid AuthorUserId { get; set; }

    /// <summary>
    /// Thứ tự ưu tiên
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Kích hoạt
    /// </summary>
    public bool IsActive { get; set; }
}