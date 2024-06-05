using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace Tag.Grpc.Entities;

[Table("Tags")]
public class TagBase : EntityAuditBase<Guid>
{
    /// <summary>
    /// Tên thẻ
    /// </summary>
    [MaxLength(50)]
    [Required]
    public required string Name { get; set; }
    
    /// <summary>
    /// Mô tả thẻ
    /// </summary>
    [MaxLength(100)]
    public string? Description { get; set; }
}