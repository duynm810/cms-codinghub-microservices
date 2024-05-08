using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;

namespace Category.API.Entities;

[Table("Categories")]
[Index(nameof(Slug), IsUnique = true)]
public class CategoryBase : EntityAuditBase<long>
{
    /// <summary>
    /// Tên danh mục
    /// </summary>
    [MaxLength(250)]
    public required string Name { get; set; }

    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) của danh mục
    /// </summary>
    [Column(TypeName = "varchar(250)")]
    public required string Slug { get; set; }

    /// <summary>
    /// Mô tả SEO, dùng cho meta description
    /// </summary>
    [MaxLength(150)]
    public string? SeoDescription { get; set; }

    /// <summary>
    /// ID danh mục cha. Nếu null, danh mục này là cấp cao nhất.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Thứ tự ưu tiên
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Kích hoạt
    /// </summary>
    public bool IsActive { get; set; }
}