using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;

namespace Tag.Api.Entities;

[Table("Tags")]
[Index(nameof(Slug), IsUnique = true)]
public class TagBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Tên thẻ
    /// </summary>
    [MaxLength(50)]
    [Required]
    public required string Name { get; set; }
    
    /// <summary>
    /// Đường dẫn tĩnh (SEO-friendly URL) của thẻ
    /// </summary>
    [Column(TypeName = "varchar(250)")]
    [Required]
    public required string Slug { get; set; }
    
    /// <summary>
    /// Mô tả thẻ
    /// </summary>
    [MaxLength(100)]
    public string? Description { get; set; }
}