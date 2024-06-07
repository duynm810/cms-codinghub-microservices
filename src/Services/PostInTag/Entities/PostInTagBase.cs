using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace PostInTag.Api.Entities;

[Table("PostInTag")]
public class PostInTagBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Khóa liên kết đến thẻ bài viết
    /// </summary>
    [Required]
    public Guid TagId { get; set; }

    /// <summary>
    /// Khóa liên kết đến bài viết
    /// </summary>
    [Required]
    public Guid PostId { get; set; }

    /// <summary>
    /// Thứ tự ưu tiên
    /// </summary>
    public int SortOrder { get; set; }
}