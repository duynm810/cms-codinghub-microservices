using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace Tag.Api.Entities;

[Table("Tags")]
public class TagBase : EntityAuditBase<Guid>
{
    public required string Name { get; set; }
    
    public string? Description { get; set; }
}