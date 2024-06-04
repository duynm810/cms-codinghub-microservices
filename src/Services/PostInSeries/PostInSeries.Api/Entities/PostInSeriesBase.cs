using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace PostInSeries.Api.Entities;

[Table("PostInSeries")]
public class PostInSeriesBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Khóa liên kết đến loạt bài viết
    /// </summary>
    [Required]
    public Guid SeriesId { get; set; }

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