using Contracts.Domains;

namespace Category.Grpc.Entities;

public class CategoryBase : EntityAuditBase<long>
{
    /// <summary>
    /// Tên danh mục
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) của danh mục
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Mô tả SEO, dùng cho meta description
    /// </summary>
    public string? SeoDescription { get; set; }

    /// <summary>
    /// ID danh mục cha. Nếu null, danh mục này là cấp cao nhất.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Biểu tượng danh mục
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Màu danh mục
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Thứ tự ưu tiên
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Phân biệt danh mục tĩnh hay động
    /// </summary>
    public bool IsStaticPage { get; set; }

    /// <summary>
    /// Kích hoạt
    /// </summary>
    public bool IsActive { get; set; }
}