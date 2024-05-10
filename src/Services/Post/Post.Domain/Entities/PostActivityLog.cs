using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Shared.Enums;

namespace Post.Domain.Entities;

[Table("PostActivityLogs")]
public class PostActivityLog : EntityAuditBase<Guid>
{
    /// <summary>
    /// Ghi chú liên quan đến hoạt động bài viết
    /// </summary>
    [MaxLength(500)] 
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái bài viết trước khi thay đổi
    /// </summary>
    public PostStatusEnum FromStatus { get; set; }

    /// <summary>
    /// Trạng thái bài viết sau khi thay đổi
    /// </summary>
    public PostStatusEnum ToStatus { get; set; }
    
    /// <summary>
    ///  Khóa ngoại thuộc về bài viết nào
    /// </summary>
    [Required]
    public Guid PostId { get; set; }

    /// <summary>
    /// Khóa ngoại người thực hiện hoạt động
    /// </summary>
    public Guid UserId { get; set; }
}